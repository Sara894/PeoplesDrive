using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Dialogue (optional)")]
    [SerializeField] private string dialogueKnotName;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;
    [SerializeField] private bool autoStartDialogueOnFinish = false;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;
    private TextMeshProUGUI interactionPromptText;

    private void Awake() 
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>(true);
        
        GameObject promptObj = GameObject.Find("Press E");
        if (promptObj != null)
        {
            interactionPromptText = promptObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning($"QuestPoint {gameObject.name}: Could not find 'Press E' GameObject in scene!");
        }
    }

    private void OnEnable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStartBlocked += OnQuestStartBlocked;
            GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
            GameEventsManager.instance.dialogueEvents.onDialogueStarted += OnDialogueStarted;
            GameEventsManager.instance.dialogueEvents.onDialogueFinished += OnDialogueFinished;
        }
    }

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStartBlocked += OnQuestStartBlocked;
            GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        }
        
        InitializeQuestState();
    }
    
    private void InitializeQuestState()
    {
        if (QuestManager.instance != null)
        {
            Quest quest = QuestManager.instance.GetQuestById(questId);
            if (quest != null)
            {
                currentQuestState = quest.state;
                Debug.Log($"QuestPoint {gameObject.name}: Initialized with quest state: {currentQuestState}");
                
                if (questIcon != null)
                {
                    questIcon.SetState(currentQuestState, startPoint, finishPoint);
                }
            }
            else
            {
                Debug.LogWarning($"QuestPoint {gameObject.name}: Quest '{questId}' not found in QuestManager!");
            }
        }
    }

    private void OnDisable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStartBlocked -= OnQuestStartBlocked;
            GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
            GameEventsManager.instance.dialogueEvents.onDialogueStarted -= OnDialogueStarted;
            GameEventsManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
        }
    }

    private void OnDialogueStarted()
    {
        HideInteractionPrompt();
    }

    private void OnDialogueFinished()
    {
        UpdateInteractionPrompt();
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        Debug.Log($"SubmitPressed - playerIsNear: {playerIsNear}, context: {inputEventContext}, currentQuestState: {currentQuestState}, dialogueKnotName: '{dialogueKnotName}'");
        
        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        if (currentQuestState == QuestState.CAN_START && startPoint)
        {
            if (QuestManager.instance.HasActiveQuest())
            {
                Debug.Log($"Cannot interact with quest '{questInfoForPoint.displayName}' - another quest is already active");
                return;
            }
        }

        StartDialogue();
    }

    private void StartDialogue()
    {
        if (!string.IsNullOrEmpty(dialogueKnotName)) 
        {
            if (GameEventsManager.instance != null && GameEventsManager.instance.dialogueEvents != null)
            {
                Debug.Log($"Starting dialogue: {dialogueKnotName}");
                GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueKnotName);
            }
            else
            {
                Debug.LogWarning($"Dialogue system not available for '{dialogueKnotName}', starting quest directly instead.");
                StartOrFinishQuest();
            }
        }
        else 
        {
            StartOrFinishQuest();
        }
    }

    private void StartOrFinishQuest()
    {
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            Debug.Log("Starting quest: " + questId);
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            Debug.Log("Finishing quest: " + questId);
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            QuestState previousState = currentQuestState;
            currentQuestState = quest.state;
            Debug.Log($"QuestPoint {gameObject.name}: Quest state changed to {currentQuestState}");
            
            if (questIcon != null)
            {
                questIcon.SetState(currentQuestState, startPoint, finishPoint);
            }
            else
            {
                Debug.LogWarning($"QuestIcon is null on {gameObject.name}!");
            }
            
            // Auto-start dialogue when quest becomes CAN_FINISH
            if (autoStartDialogueOnFinish && finishPoint && 
                previousState != QuestState.CAN_FINISH && 
                currentQuestState == QuestState.CAN_FINISH)
            {
                Debug.Log($"QuestPoint {gameObject.name}: Quest became CAN_FINISH, auto-starting dialogue after brief delay");
                StartCoroutine(AutoStartDialogueAfterDelay(0.5f));
            }
            
            UpdateInteractionPrompt();
        }
    }

    private void OnQuestStartBlocked(string blockedQuestId)
    {
        if (blockedQuestId.Equals(questId) && playerIsNear)
        {
            Debug.Log($"Quest '{questInfoForPoint.displayName}' blocked - another quest is active!");
            ShowBlockedMessage();
        }
    }

    private void ShowBlockedMessage()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = "Complete your active quest first!";
            interactionPromptText.gameObject.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }

    private System.Collections.IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideInteractionPrompt();
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        Debug.Log($"QuestPoint OnTriggerEnter: {otherCollider.name}, Tag: {otherCollider.tag}");
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log($"Player is near! Current quest state: {currentQuestState}");
            UpdateInteractionPrompt();
            
            // Auto-start dialogue when entering finish zone if quest is ready to finish
            if (autoStartDialogueOnFinish && finishPoint && currentQuestState == QuestState.CAN_FINISH)
            {
                Debug.Log($"QuestPoint {gameObject.name}: Auto-starting dialogue at finish point");
                StartDialogue();
            }
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
            Debug.Log("Player left quest area");
            HideInteractionPrompt();
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (interactionPromptText == null) return;

        if (playerIsNear && currentQuestState == QuestState.CAN_START && startPoint)
        {
            if (QuestManager.instance.HasActiveQuest())
            {
                HideInteractionPrompt();
                return;
            }
            
            interactionPromptText.text = $"Press E to start {questInfoForPoint.displayName}";
            interactionPromptText.gameObject.SetActive(true);
        }
        else if (playerIsNear && currentQuestState == QuestState.CAN_FINISH && finishPoint)
        {
            if (!autoStartDialogueOnFinish)
            {
                interactionPromptText.text = $"Press E to finish {questInfoForPoint.displayName}";
                interactionPromptText.gameObject.SetActive(true);
            }
            else
            {
                HideInteractionPrompt();
            }
        }
        else
        {
            HideInteractionPrompt();
        }
    }

    private void HideInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    private IEnumerator AutoStartDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"QuestPoint {gameObject.name}: Auto-starting dialogue now");
        StartDialogue();
    }
}
