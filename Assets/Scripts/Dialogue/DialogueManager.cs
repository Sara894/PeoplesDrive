using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    [Header("Auto Continue Settings")]
    [SerializeField] private bool autoCloseOnLastLine = true;
    [SerializeField] private float autoCloseDelay = 1.5f;
    [SerializeField] private bool waitForQuestProcessing = true;

    private Story story;
    private int currentChoiceIndex = -1;
    private bool choicesBeingDisplayed = false;
    private bool canConfirmChoice = false;

    private bool dialoguePlaying = false;
    private bool pendingAutoClose = false;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;
    private Coroutine autoCloseCoroutine;

    private void Awake() 
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy() 
    {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable() 
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance == null)
        {
            Debug.LogWarning("DialogueManager: GameEventsManager.instance is null, cannot subscribe to events");
            return;
        }
            
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        
        Debug.Log("DialogueManager: Successfully subscribed to events");
    }

    private void OnDisable() 
    {
        if (GameEventsManager.instance == null)
            return;
            
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest) 
    {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            new StringValue(quest.state.ToString())
        );

        // If we're waiting to auto-close after quest processing, start the timer now
        if (pendingAutoClose && waitForQuestProcessing)
        {
            Debug.Log($"DialogueManager: Quest state changed to {quest.state}, starting auto-close timer");
            pendingAutoClose = false;
            autoCloseCoroutine = StartCoroutine(AutoCloseAfterDelay());
        }
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) 
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex) 
    {
        Debug.Log($"<color=cyan>DialogueManager: UpdateChoiceIndex called with index: {choiceIndex}</color>");
        this.currentChoiceIndex = choiceIndex;
        // Only update the index - don't auto-continue
        // Continue happens when player presses E/Enter or clicks
    }

    private void SubmitPressed(InputEventContext inputEventContext) 
    {
        Debug.Log($"<color=cyan>DialogueManager: SubmitPressed - context: {inputEventContext}, currentChoiceIndex: {currentChoiceIndex}, choicesBeingDisplayed: {choicesBeingDisplayed}, canConfirmChoice: {canConfirmChoice}</color>");
        
        // if the context isn't dialogue, we never want to register input here
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE)) 
        {
            Debug.Log("DialogueManager: Context not DIALOGUE, ignoring");
            return;
        }

        // If choices are being displayed, confirm the current choice
        if (choicesBeingDisplayed)
        {
            // Block input until at least one frame after choices appeared
            if (!canConfirmChoice)
            {
                Debug.Log("<color=yellow>DialogueManager: Choices just appeared, ignoring E press. Wait a moment before confirming.</color>");
                return;
            }

            if (currentChoiceIndex != -1)
            {
                Debug.Log($"<color=green>DialogueManager: Confirming choice {currentChoiceIndex} with E/Enter</color>");
                choicesBeingDisplayed = false;
                canConfirmChoice = false;
                ContinueOrExitStory();
            }
            else
            {
                Debug.LogWarning("DialogueManager: No choice selected! Navigate with arrow keys first.");
            }
            return;
        }

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName) 
    {
        Debug.Log($"<color=orange>DialogueManager: EnterDialogue called with knotName: '{knotName}'</color>");
        
        InputEventContext currentContext = GameEventsManager.instance.inputEvents.inputEventContext;
        
        if (currentContext == InputEventContext.QUEST_LOG)
        {
            Debug.LogWarning("DialogueManager: Cannot start dialogue while Quest Log is open");
            return;
        }

        if (currentContext == InputEventContext.TUTORIAL)
        {
            Debug.LogWarning("DialogueManager: Cannot start dialogue while Tutorial is open");
            return;
        }

        if (dialoguePlaying) 
        {
            Debug.LogWarning("DialogueManager: Already in dialogue, ignoring EnterDialogue call");
            return;
        }

        dialoguePlaying = true;

        GameEventsManager.instance.dialogueEvents.DialogueStarted();
        Debug.Log("DialogueManager: Fired DialogueStarted event");

        GameEventsManager.instance.playerEvents.DisablePlayerMovement();

        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else 
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue.");
        }

        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() 
    {
        Debug.Log($"<color=yellow>DialogueManager: ContinueOrExitStory - canContinue: {story.canContinue}, choices: {story.currentChoices.Count}, currentChoiceIndex: {currentChoiceIndex}</color>");
        
        // make a choice, if applicable
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            Debug.Log($"DialogueManager: Choosing choice index {currentChoiceIndex}");
            story.ChooseChoiceIndex(currentChoiceIndex);
            // reset choice index for next time
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            // handle the case where there's an empty line of dialogue
            // by continuing until we get a line with content
            while (IsLineBlank(dialogueLine) && story.canContinue) 
            {
                dialogueLine = story.Continue();
            }
            // handle the case where the last line of dialogue is blank
            // (empty choice, external function, etc...)
            if (IsLineBlank(dialogueLine) && !story.canContinue) 
            {
                ExitDialogue();
            }
            else 
            {
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
                
                // Check if choices are being displayed
                if (story.currentChoices.Count > 0)
                {
                    Debug.Log($"<color=magenta>DialogueManager: Displaying {story.currentChoices.Count} choices - blocking auto-continue</color>");
                    choicesBeingDisplayed = true;
                    canConfirmChoice = false;
                    // Enable choice confirmation after a short delay to prevent accidental instant-confirm
                    StartCoroutine(EnableChoiceConfirmationAfterDelay());
                }
                else
                {
                    choicesBeingDisplayed = false;
                    canConfirmChoice = false;
                }
                
                // Auto-close if this is the last line with no choices
                if (autoCloseOnLastLine && !story.canContinue && story.currentChoices.Count == 0)
                {
                    if (waitForQuestProcessing)
                    {
                        // Mark that we want to auto-close, but wait for quest state change first
                        Debug.Log("DialogueManager: Last line detected, waiting for quest processing before auto-closing");
                        pendingAutoClose = true;
                    }
                    else
                    {
                        // Start auto-close immediately
                        Debug.Log($"DialogueManager: Last line with no choices detected, auto-closing in {autoCloseDelay}s");
                        autoCloseCoroutine = StartCoroutine(AutoCloseAfterDelay());
                    }
                }
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        // Cancel auto-close if it's running
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }

        // Reset pending auto-close flag
        pendingAutoClose = false;

        // Reset choice flags
        choicesBeingDisplayed = false;
        canConfirmChoice = false;
        currentChoiceIndex = -1;

        dialoguePlaying = false;

        // inform other parts of our system that we've finished dialogue
        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        // let player move again
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();

        // input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        // stop listening for dialogue variables
        inkDialogueVariables.StopListening(story);

        // reset story state
        story.ResetState();
    }

    private System.Collections.IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        Debug.Log("DialogueManager: Auto-closing dialogue");
        ExitDialogue();
    }

    private System.Collections.IEnumerator EnableChoiceConfirmationAfterDelay()
    {
        // Wait for next frame to ensure the E press that opened dialogue is consumed
        yield return null;
        canConfirmChoice = true;
        Debug.Log("<color=green>DialogueManager: Choice confirmation now enabled</color>");
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
