using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestLogUI : MonoBehaviour
{
      [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI goldRewardsText;
    [SerializeField] private TextMeshProUGUI experienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;
    [SerializeField] private GameObject acceptButton;

    private Button firstSelectedButton;
    private Quest currentlySelectedQuest;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onQuestLogTogglePressed += QuestLogTogglePressed;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;

        Button acceptBtn = acceptButton.GetComponent<Button>();
        if (acceptBtn != null)
        {
            acceptBtn.onClick.AddListener(OnAcceptButtonClicked);
        }
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onQuestLogTogglePressed -= QuestLogTogglePressed;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;

        Button acceptBtn = acceptButton.GetComponent<Button>();
        if (acceptBtn != null)
        {
            acceptBtn.onClick.RemoveListener(OnAcceptButtonClicked);
        }
    }

    private void QuestLogTogglePressed()
    {
        InputEventContext currentContext = GameEventsManager.instance.inputEvents.inputEventContext;

        if (currentContext == InputEventContext.DIALOGUE)
        {
            Debug.Log("QuestLogUI: Cannot open Quest Log while in dialogue");
            return;
        }

        if (currentContext == InputEventContext.TUTORIAL)
        {
            Debug.Log("QuestLogUI: Cannot open Quest Log while in tutorial");
            return;
        }

        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.QUEST_LOG);
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        
        acceptButton.SetActive(false);
        
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        EventSystem.current.SetSelectedGameObject(null);
        
        acceptButton.SetActive(false);
    }

    private void QuestStateChange(Quest quest)
    {
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => {
            SetQuestLogInfo(quest);
        });

        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }

        questLogButton.SetState(quest.state);
        
        if (currentlySelectedQuest != null && contentParent.activeInHierarchy)
        {
            SetQuestLogInfo(currentlySelectedQuest);
        }
    }

    private void SetQuestLogInfo(Quest quest)
    {
        currentlySelectedQuest = quest;

        questDisplayNameText.text = quest.info.displayName;

        questStatusText.text = quest.GetFullStatusText();

        levelRequirementsText.text = "Level " + quest.info.levelRequiredToUnlock;
        questRequirementsText.text = "";
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        }

        goldRewardsText.text = quest.info.moneyReward + " Gold";
        experienceRewardsText.text = quest.info.experienceCommunityReward + " XP";

        bool canAccept = (quest.state == QuestState.CAN_START) && !QuestManager.instance.HasActiveQuest();
        acceptButton.SetActive(canAccept);
        
        if (quest.state == QuestState.CAN_START && QuestManager.instance.HasActiveQuest())
        {
            questStatusText.text += "\n\n<color=red>Complete your active quest first!</color>";
        }
    }

    private void OnAcceptButtonClicked()
    {
        if (currentlySelectedQuest == null)
        {
            Debug.LogWarning("QuestLogUI: No quest selected when Accept was clicked!");
            return;
        }

        QuestHUDDisplay hudDisplay = FindObjectOfType<QuestHUDDisplay>();
        if (hudDisplay != null)
        {
            hudDisplay.DisplayAcceptedQuest(currentlySelectedQuest, "Go to location to start quest");
        }
        else
        {
            Debug.LogWarning("QuestLogUI: Could not find QuestHUDDisplay in scene!");
        }

        QuestArrowPointer[] allArrowPointers = FindObjectsOfType<QuestArrowPointer>(true);
        QuestArrowPointer arrowPointer = null;
        
        foreach (QuestArrowPointer pointer in allArrowPointers)
        {
            if (pointer.gameObject.activeInHierarchy)
            {
                arrowPointer = pointer;
                break;
            }
        }
        
        if (arrowPointer != null)
        {
            arrowPointer.ActivateArrowForQuest(currentlySelectedQuest);
        }
        else
        {
            Debug.LogWarning("QuestLogUI: Could not find active QuestArrowPointer in scene!");
        }

        HideUI();
    }
}
