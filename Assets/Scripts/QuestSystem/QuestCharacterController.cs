using UnityEngine;

public class QuestCharacterController : MonoBehaviour
{
    [Header("Character Reference")]
    [Tooltip("Enable/disable the entire character appearance system for this quest point")]
    [SerializeField] private bool enableCharacterSystem = true;
    
    [SerializeField] private GameObject characterToControl;
    
    [Header("Quest Reference")]
    [SerializeField] private QuestInfoSO questInfo;
    
    [Header("Character Behavior")]
    [Tooltip("Show character when dialogue starts (when pressing E)")]
    [SerializeField] private bool showOnDialogueStart = false;
    
    [Tooltip("Hide character when dialogue ends without accepting quest (when declining or closing)")]
    [SerializeField] private bool hideOnDialogueEnd = false;
    
    [Tooltip("Show character when quest starts (after pressing E)")]
    [SerializeField] private bool showOnQuestStart = false;
    
    [Tooltip("Hide character when quest starts (after accepting in dialogue)")]
    [SerializeField] private bool hideOnQuestStart = false;
    
    [Tooltip("Hide character when player leaves the zone")]
    [SerializeField] private bool hideOnPlayerExit = false;
    
    [Tooltip("Hide character when player leaves zone AFTER quest is finished")]
    [SerializeField] private bool hideOnPlayerExitAfterFinish = false;
    
    [Tooltip("Delay in seconds before hiding character after showing (for future dialogue)")]
    [SerializeField] private float hideDelayAfterShow = 0f;
    
    [Tooltip("Show character when player can finish quest")]
    [SerializeField] private bool showOnCanFinish = false;
    
    [Tooltip("Show character immediately when quest finishes (before hide)")]
    [SerializeField] private bool showOnQuestFinish = false;
    
    [Tooltip("Hide character when quest finishes (delivery complete)")]
    [SerializeField] private bool hideOnQuestFinish = false;
    
    [Tooltip("Minimum seconds character must stay visible before hiding (prevents instant despawn)")]
    [SerializeField] private float minDisplayDuration = 0f;

    private string questId;
    private QuestState currentQuestState;
    private Coroutine hideCoroutine;
    private float characterShownTime = 0f;
    private bool playerInZone = false;

    private void Awake()
    {
        if (questInfo != null)
        {
            questId = questInfo.id;
        }
        
        if (characterToControl != null)
        {
            characterToControl.SetActive(false);
        }
    }

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += OnQuestStateChange;
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= OnQuestStateChange;
            
            if (playerInZone)
            {
                GameEventsManager.instance.dialogueEvents.onDialogueStarted -= OnDialogueStarted;
                GameEventsManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enableCharacterSystem)
            return;
            
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            
            if (GameEventsManager.instance != null)
            {
                GameEventsManager.instance.dialogueEvents.onDialogueStarted += OnDialogueStarted;
                GameEventsManager.instance.dialogueEvents.onDialogueFinished += OnDialogueFinished;
            }
            
            Debug.Log($"QuestCharacterController ({gameObject.name}): Player entered zone. Current quest state: {currentQuestState}");
            
            if (showOnCanFinish && currentQuestState == QuestState.CAN_FINISH)
            {
                Debug.Log($"QuestCharacterController ({gameObject.name}): Conditions met to show character on player entry (showOnCanFinish && CAN_FINISH)");
                ShowCharacter();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enableCharacterSystem)
            return;
            
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            
            if (GameEventsManager.instance != null)
            {
                GameEventsManager.instance.dialogueEvents.onDialogueStarted -= OnDialogueStarted;
                GameEventsManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
            }
            
            Debug.Log($"QuestCharacterController ({gameObject.name}): Player left zone");
            
            bool shouldHide = false;
            
            if (hideOnPlayerExit)
            {
                shouldHide = true;
                Debug.Log($"QuestCharacterController ({gameObject.name}): Hiding character - hideOnPlayerExit is enabled");
            }
            else if (hideOnPlayerExitAfterFinish && currentQuestState == QuestState.FINISHED)
            {
                shouldHide = true;
                Debug.Log($"QuestCharacterController ({gameObject.name}): Hiding character - quest is finished and hideOnPlayerExitAfterFinish is enabled");
            }
            
            if (shouldHide && characterToControl != null && characterToControl.activeSelf)
            {
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                    hideCoroutine = null;
                }
                HideCharacter();
            }
        }
    }

    public void TriggerCharacterShow()
    {
        if (!enableCharacterSystem)
        {
            Debug.Log($"QuestCharacterController ({gameObject.name}): Character system disabled, skipping show");
            return;
        }
        
        Debug.Log($"<color=yellow>QuestCharacterController ({gameObject.name}): Manually triggered character show</color>");
        ShowCharacter();
    }

    private void OnDialogueStarted()
    {
        if (!enableCharacterSystem)
            return;

        if (showOnDialogueStart)
        {
            Debug.Log($"<color=magenta>QuestCharacterController ({gameObject.name}): Dialogue started, showing character</color>");
            ShowCharacter();
        }
    }

    private void OnDialogueFinished()
    {
        if (!enableCharacterSystem)
            return;

        if (hideOnDialogueEnd)
        {
            if (currentQuestState == QuestState.CAN_START || currentQuestState == QuestState.REQUIREMENTS_NOT_MET)
            {
                Debug.Log($"<color=magenta>QuestCharacterController ({gameObject.name}): Dialogue ended without accepting quest (state: {currentQuestState}), hiding character</color>");
                HideCharacter();
            }
            else
            {
                Debug.Log($"QuestCharacterController ({gameObject.name}): Dialogue ended but quest is in progress/finished (state: {currentQuestState}), not hiding");
            }
        }
    }

    private void OnQuestStateChange(Quest quest)
    {
        if (!enableCharacterSystem)
            return;
            
        if (quest.info.id != questId)
            return;

        QuestState previousState = currentQuestState;
        currentQuestState = quest.state;

        Debug.Log($"<color=cyan>QuestCharacterController ({gameObject.name}): Quest '{questId}' state changed from {previousState} to {currentQuestState}</color>");

        if (currentQuestState == QuestState.IN_PROGRESS && previousState == QuestState.CAN_START)
        {
            Debug.Log($"QuestCharacterController: Quest started. showOnQuestStart={showOnQuestStart}, hideOnQuestStart={hideOnQuestStart}");
            
            if (showOnQuestStart)
            {
                ShowCharacter();
                
                if (hideDelayAfterShow > 0f)
                {
                    if (hideCoroutine != null)
                    {
                        StopCoroutine(hideCoroutine);
                    }
                    hideCoroutine = StartCoroutine(HideCharacterAfterDelay(hideDelayAfterShow));
                }
            }
            
            if (hideOnQuestStart)
            {
                HideCharacter();
            }
        }

        if (currentQuestState == QuestState.CAN_FINISH)
        {
            Debug.Log($"QuestCharacterController: Quest can finish. showOnCanFinish={showOnCanFinish}, playerInZone={playerInZone}");
            if (showOnCanFinish && playerInZone)
            {
                ShowCharacter();
            }
        }

        if (currentQuestState == QuestState.FINISHED)
        {
            Debug.Log($"QuestCharacterController: Quest finished. showOnQuestFinish={showOnQuestFinish}, hideOnQuestFinish={hideOnQuestFinish}");
            
            if (showOnQuestFinish)
            {
                ShowCharacter();
            }
            
            if (hideOnQuestFinish)
            {
                HideCharacterWithMinDuration();
            }
        }
    }

    private void ShowCharacter()
    {
        if (characterToControl != null)
        {
            bool wasActive = characterToControl.activeSelf;
            characterToControl.SetActive(true);
            characterShownTime = Time.time;
            Debug.Log($"<color=green>QuestCharacterController: SHOWING character '{characterToControl.name}' (was active: {wasActive})</color>");
        }
        else
        {
            Debug.LogError($"QuestCharacterController ({gameObject.name}): characterToControl is NULL!");
        }
    }

    private void HideCharacter()
    {
        if (characterToControl != null)
        {
            bool wasActive = characterToControl.activeSelf;
            characterToControl.SetActive(false);
            Debug.Log($"<color=red>QuestCharacterController: HIDING character '{characterToControl.name}' (was active: {wasActive})</color>");
        }
    }

    private void HideCharacterWithMinDuration()
    {
        if (characterToControl != null && characterToControl.activeSelf)
        {
            float timeSinceShown = Time.time - characterShownTime;
            float remainingTime = Mathf.Max(0f, minDisplayDuration - timeSinceShown);
            
            Debug.Log($"QuestCharacterController: Character shown for {timeSinceShown:F2}s, minDisplayDuration={minDisplayDuration}s, will hide in {remainingTime:F2}s");
            
            if (remainingTime > 0f)
            {
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                }
                hideCoroutine = StartCoroutine(HideCharacterAfterDelay(remainingTime));
            }
            else
            {
                HideCharacter();
            }
        }
        else
        {
            Debug.Log($"QuestCharacterController: Character already inactive or null, skipping hide");
        }
    }

    private System.Collections.IEnumerator HideCharacterAfterDelay(float delay)
    {
        Debug.Log($"QuestCharacterController: Waiting {delay:F2}s before hiding character");
        yield return new WaitForSeconds(delay);
        HideCharacter();
    }
}
