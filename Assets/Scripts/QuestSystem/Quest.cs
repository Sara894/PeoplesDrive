using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO info;

    public QuestState state;
    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    public bool isFailing { get; private set; }

    public Quest(QuestInfoSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.isFailing = false;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        this.isFailing = false;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.info.id);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public int GetCurrentQuestStepIndex()
    {
        return currentQuestStepIndex;
    }

    public int GetCurrentStepIndex()
    {
        return currentQuestStepIndex;
    }

    public void ResetQuest()
    {
        currentQuestStepIndex = 0;
        isFailing = false;
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
        Debug.Log($"Quest {info.id} has been reset to initial state");
    }

    public void MarkAsFailing()
    {
        isFailing = true;
        Debug.Log($"Quest {info.id} marked as failing");
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        Debug.Log($"InstantiateCurrentQuestStep for {info.id}: prefab={questStepPrefab?.name}, stepIndex={currentQuestStepIndex}");
        
        if (questStepPrefab != null)
        {
            GameObject existingQuestStep = FindExistingQuestStepInScene(questStepPrefab.name);
            
            if (existingQuestStep != null)
            {
                Debug.Log($"Found existing quest step in scene: {existingQuestStep.name}, using it instead of instantiating");
                QuestStep questStep = existingQuestStep.GetComponent<QuestStep>();
                
                if (questStep != null)
                {
                    existingQuestStep.SetActive(true);
                    questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
                    Debug.Log($"Activated and initialized existing quest step: {existingQuestStep.name}");
                }
                else
                {
                    Debug.LogError($"Found GameObject {existingQuestStep.name} but it has no QuestStep component!");
                }
            }
            else
            {
                QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform)
                    .GetComponent<QuestStep>();
                Debug.Log($"Instantiated quest step: {questStep.name}, initializing...");
                questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
            }
        }
        else
        {
            Debug.LogWarning($"Quest step prefab is null for {info.id} at step index {currentQuestStepIndex}");
        }
    }

    private GameObject FindExistingQuestStepInScene(string prefabName)
    {
        GameObject exactMatch = GameObject.Find(prefabName);
        if (exactMatch != null)
        {
            return exactMatch;
        }

        string prefabNameWithoutVariant = prefabName.Replace(" Variant", "").Trim();

        GameObject variantMatch = GameObject.Find(prefabNameWithoutVariant);
        if (variantMatch != null)
        {
            Debug.Log($"Found GameObject '{variantMatch.name}' matching prefab variant '{prefabName}'");
            return variantMatch;
        }

        QuestStep[] allQuestSteps = Object.FindObjectsOfType<QuestStep>(true);
        foreach (QuestStep questStep in allQuestSteps)
        {
            string stepName = questStep.gameObject.name;
            if (stepName.StartsWith(prefabName) && !stepName.Contains("(Clone)"))
            {
                return questStep.gameObject;
            }
            
            if (stepName.StartsWith(prefabNameWithoutVariant) && !stepName.Contains("(Clone)"))
            {
                Debug.Log($"Found GameObject '{stepName}' matching prefab variant '{prefabName}' (without Variant suffix)");
                return questStep.gameObject;
            }
        }

        return null;
    }

    public GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else 
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                + "there's no current step: QuestId=" + info.id + ", stepIndex=" + currentQuestStepIndex);
        }
        return questStepPrefab;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
            questStepStates[stepIndex].status = questStepState.status;
        }
        else 
        {
            Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range: "
                + "Quest Id = " + info.id + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }

    public string GetFullStatusText()
    {
        string fullStatus = "";

        if (state == QuestState.REQUIREMENTS_NOT_MET)
        {
            fullStatus = "Requirements are not yet met to start this quest.";
        }
        else if (state == QuestState.CAN_START)
        {
            fullStatus = "This quest can be started!";
        }
        else 
        {
            // display all previous quests with strikethroughs
            for (int i = 0; i < currentQuestStepIndex; i++)
            {
                fullStatus += "<s>" + questStepStates[i].status + "</s>\n";
            }
            // display the current step, if it exists
            if (CurrentStepExists())
            {
                fullStatus += questStepStates[currentQuestStepIndex].status;
            }
            // when the quest is completed or turned in
            if (state == QuestState.CAN_FINISH)
            {
                fullStatus += "The quest is ready to be turned in.";
            }
            else if (state == QuestState.FINISHED)
            {
                fullStatus += "The quest has been completed!";
            }
        }

        return fullStatus;
    }
    
}
