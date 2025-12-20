using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestHUDDisplay : MonoBehaviour
{
    [Header("HUD Text Components")]
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Display Settings")]
    [SerializeField] private bool hideWhenNoActiveQuest = true;
    [SerializeField] private string[] interactionKeywords = { "Press E", "Press", "[E]" };

    private Quest currentActiveQuest;
    private string lastDescription = "";

    private void OnEnable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
        }
    }

    private void OnDisable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
        }
    }

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
            GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
        }
        
        if (hideWhenNoActiveQuest)
        {
            HideQuestDisplay();
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.state == QuestState.IN_PROGRESS)
        {
            currentActiveQuest = quest;
            UpdateQuestDisplay();
            ShowQuestDisplay();
        }
        else if (quest.state == QuestState.CAN_FINISH)
        {
            currentActiveQuest = quest;
            UpdateQuestDisplay();
        }
        else if (quest.state == QuestState.FINISHED && currentActiveQuest != null && quest.info.id == currentActiveQuest.info.id)
        {
            currentActiveQuest = quest;
            UpdateQuestDisplay();
            
            if (hideWhenNoActiveQuest)
            {
                Invoke(nameof(ClearAndHideQuest), 2f);
            }
        }
        else if (quest.state == QuestState.CAN_START && currentActiveQuest != null && quest.info.id == currentActiveQuest.info.id)
        {
            Debug.Log($"QuestHUDDisplay: Quest '{quest.info.displayName}' returned to CAN_START (failed). Hiding display.");
            ClearAndHideQuest();
        }
    }

    public void DisplayAcceptedQuest(Quest quest, string customDescription)
    {
        currentActiveQuest = quest;
        
        if (questNameText != null)
        {
            questNameText.text = quest.info.displayName;
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = customDescription;
        }

        if (questStatusText != null)
        {
            questStatusText.text = GetQuestStateDisplayText(quest.state);
        }

        ShowQuestDisplay();
        
        Debug.Log($"QuestHUDDisplay: Showing accepted quest - {quest.info.displayName}");
    }

    private void ClearAndHideQuest()
    {
        currentActiveQuest = null;
        HideQuestDisplay();
    }

    private void QuestStepStateChange(string questId, int stepIndex, QuestStepState questStepState)
    {
        if (currentActiveQuest != null && currentActiveQuest.info.id == questId)
        {
            UpdateQuestDisplay();
        }
    }

    private void UpdateQuestDisplay()
    {
        if (currentActiveQuest == null)
        {
            ClearQuestDisplay();
            return;
        }

        if (questNameText != null)
        {
            questNameText.text = currentActiveQuest.info.displayName;
        }

        string currentDescription = currentActiveQuest.GetFullStatusText();
        
        if (questDescriptionText != null)
        {
            questDescriptionText.text = currentDescription;
        }

        if (questStatusText != null)
        {
            questStatusText.text = GetQuestStateDisplayText(currentActiveQuest.state);
        }

        UpdateInteractionPrompt(currentDescription);
        lastDescription = currentDescription;
    }

    private string GetQuestStateDisplayText(QuestState state)
    {
        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                return "Requirements Not Met";
            case QuestState.CAN_START:
                return "Available";
            case QuestState.IN_PROGRESS:
                return "In Progress";
            case QuestState.CAN_FINISH:
                return "Can Finish";
            case QuestState.FINISHED:
                return "Finished";
            default:
                return "";
        }
    }

    private void UpdateInteractionPrompt(string description)
    {
        if (interactionPromptText == null)
            return;

        bool shouldShowPrompt = false;
        string promptText = "";

        foreach (string keyword in interactionKeywords)
        {
            if (description.Contains(keyword))
            {
                shouldShowPrompt = true;
                promptText = description;
                break;
            }
        }

        if (shouldShowPrompt && description != lastDescription)
        {
            interactionPromptText.text = promptText;
            interactionPromptText.gameObject.SetActive(true);
        }
        else if (!shouldShowPrompt)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void ClearQuestDisplay()
    {
        if (questNameText != null)
        {
            questNameText.text = "";
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = "";
        }

        if (questStatusText != null)
        {
            questStatusText.text = "";
        }

        if (interactionPromptText != null)
        {
            interactionPromptText.text = "";
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void ShowQuestDisplay()
    {
        if (questNameText != null)
        {
            questNameText.gameObject.SetActive(true);
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.gameObject.SetActive(true);
        }

        if (questStatusText != null)
        {
            questStatusText.gameObject.SetActive(true);
        }
    }

    private void HideQuestDisplay()
    {
        if (questNameText != null)
        {
            questNameText.gameObject.SetActive(false);
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.gameObject.SetActive(false);
        }

        if (questStatusText != null)
        {
            questStatusText.gameObject.SetActive(false);
        }

        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }
}
