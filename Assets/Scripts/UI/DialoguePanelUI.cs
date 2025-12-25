using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialoguePanelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;
    [SerializeField] private GameObject continueIcon;

    private void Awake()
    {
        contentParent.SetActive(false);
        ResetPanel();
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance == null)
        {
            Debug.LogWarning("DialoguePanelUI: GameEventsManager.instance is null, cannot subscribe to events");
            return;
        }
            
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;
        
        Debug.Log("DialoguePanelUI: Successfully subscribed to events");
    }

    private void OnDisable()
    {
        if (GameEventsManager.instance == null)
            return;
            
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;
    }

    private void DialogueStarted()
    {
        contentParent.SetActive(true);
    }

    private void DialogueFinished()
    {
        contentParent.SetActive(false);

        // reset anything for next time
        ResetPanel();
    }

    private void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices)
    {
        dialogueText.text = dialogueLine;

        // defensive check - if there are more choices coming in than we can support, log an error
        if (dialogueChoices.Count > choiceButtons.Length) 
        {
            Debug.LogError("More dialogue choices ("
                + dialogueChoices.Count + ") came through than are supported ("
                + choiceButtons.Length + ").");
        }

        // If there are no choices, show the continue icon
        if (dialogueChoices.Count == 0)
        {
            if (continueIcon != null)
            {
                continueIcon.SetActive(true);
            }
            
            // Hide all choice buttons
            foreach (DialogueChoiceButton choiceButton in choiceButtons) 
            {
                choiceButton.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide continue icon when showing choices
            if (continueIcon != null)
            {
                continueIcon.SetActive(false);
            }

            // start with all of the choice buttons hidden
            foreach (DialogueChoiceButton choiceButton in choiceButtons) 
            {
                choiceButton.gameObject.SetActive(false);
            }

            // enable and set info for buttons depending on ink choice information
            int choiceButtonIndex = dialogueChoices.Count - 1;
            for (int inkChoiceIndex = 0; inkChoiceIndex < dialogueChoices.Count; inkChoiceIndex++)
            {
                Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
                DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

                choiceButton.gameObject.SetActive(true);
                choiceButton.SetChoiceText(dialogueChoice.text);
                choiceButton.SetChoiceIndex(inkChoiceIndex);

                // Auto-select first choice for keyboard navigation
                if (inkChoiceIndex == 0)
                {
                    choiceButton.SelectButton();
                }

                choiceButtonIndex--;
            }
        }
    }

    private void ResetPanel()
    {
        dialogueText.text = "";
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }
    }
}
