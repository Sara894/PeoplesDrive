using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; }

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    [SerializeField] private bool saveQuestState = true;

    private Dictionary<string, Quest> questMap;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        questMap = CreateQuestMap();
    }

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            Debug.Log("QuestManager: Subscribing to quest events in Start");
            GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
            GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
            GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;
            GameEventsManager.instance.questEvents.onQuestFailed += FailQuest;
            GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
        }
        else
        {
            Debug.LogError("QuestManager: GameEventsManager.instance is NULL in Start!");
        }
        
        foreach (Quest quest in questMap.Values)
        {
            Debug.Log($"Quest loaded: {quest.info.id}, Initial State: {quest.state}");
            
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
        }
        
        StartCoroutine(BroadcastQuestStatesAfterDelay());
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
            GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
            GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
            GameEventsManager.instance.questEvents.onQuestFailed -= FailQuest;
            GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
        }
    }

    private IEnumerator BroadcastQuestStatesAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        
        foreach (Quest quest in questMap.Values)
        {
            Debug.Log($"Broadcasting quest state: {quest.info.id} = {quest.state}");
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                Debug.Log($"Failed prerequisite: {prerequisiteQuestInfo.id} not finished");
                break;
            }
        }

        Debug.Log($"Requirements met for {quest.info.id}: {meetsRequirements}");
        return meetsRequirements;
    }

    private void Update()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    public bool HasActiveQuest()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                return true;
            }
        }
        return false;
    }

    private void StartQuest(string id) 
    {
        if (HasActiveQuest())
        {
            Debug.Log($"Cannot start quest '{id}' - another quest is already active!");
            GameEventsManager.instance.questEvents.QuestStartBlocked(id);
            return;
        }

        Debug.Log($"StartQuest called for: {id}");
        Quest quest = GetQuestById(id);
        
        ReEnableQuestSteps(id);
        
        Debug.Log($"Instantiating quest step for: {id}");
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }
    
    private void ReEnableQuestSteps(string questId)
    {
        Quest quest = GetQuestById(questId);
        if (quest == null || quest.info == null || quest.info.questStepPrefabs == null)
        {
            Debug.LogWarning($"ReEnableQuestSteps: Cannot re-enable steps for quest {questId} - quest or prefabs not found");
            return;
        }

        QuestStep[] allQuestSteps = FindObjectsOfType<QuestStep>(true);
        
        for (int stepIndex = 0; stepIndex < quest.info.questStepPrefabs.Length; stepIndex++)
        {
            GameObject prefab = quest.info.questStepPrefabs[stepIndex];
            if (prefab == null) continue;
            
            string prefabName = prefab.name;
            string prefabNameWithoutVariant = prefabName.Replace(" Variant", "").Trim();
            
            foreach (QuestStep questStep in allQuestSteps)
            {
                if (questStep.gameObject.name.Contains("(Clone)")) continue;
                
                string stepName = questStep.gameObject.name;
                bool isMatch = stepName == prefabName || 
                               stepName == prefabNameWithoutVariant ||
                               stepName.StartsWith(prefabName) || 
                               stepName.StartsWith(prefabNameWithoutVariant);
                
                if (isMatch)
                {
                    questStep.gameObject.SetActive(true);
                    questStep.InitializeQuestStep(questId, stepIndex, "");
                    Debug.Log($"Re-enabled and initialized scene-placed quest step: {questStep.gameObject.name} at step index {stepIndex}");
                    break;
                }
            }
        }
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        if (quest.isFailing)
        {
            Debug.Log($"Quest {id} is failing - advancement blocked");
            return;
        }

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            // All steps completed - mark as CAN_FINISH instead of auto-finishing
            // This allows dialogue/interaction to trigger the final completion
            Debug.Log($"All steps completed for {id}, quest is now ready to finish (CAN_FINISH state)");
            ChangeQuestState(id, QuestState.CAN_FINISH);
        }
    }

    public void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);

        if (quest.isFailing)
        {
            Debug.Log($"Quest {id} is failing - finish blocked");
            return;
        }

        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);

        CheckAllQuestsCompleted();
    }

    private void CheckAllQuestsCompleted()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state != QuestState.FINISHED)
            {
                return;
            }
        }

        Debug.Log("All quests completed!");
        GameEventsManager.instance.questEvents.AllQuestsCompleted();
    }

    private void FailQuest(string id)
    {
        Debug.Log($"QuestManager.FailQuest called for: {id}");
        Quest quest = GetQuestById(id);

        foreach (Transform child in transform)
        {
            QuestStep questStep = child.GetComponent<QuestStep>();
            if (questStep != null && questStep.questId == id)
            {
                Destroy(child.gameObject);
                Debug.Log($"Destroyed instantiated quest step for failed quest: {id}");
            }
        }
        
        QuestStep[] allQuestSteps = FindObjectsOfType<QuestStep>();
        foreach (QuestStep questStep in allQuestSteps)
        {
            if (questStep.questId == id && !questStep.gameObject.name.Contains("(Clone)"))
            {
                questStep.gameObject.SetActive(false);
                Debug.Log($"Disabled scene-placed quest step: {questStep.gameObject.name}");
            }
        }

        ItemSpawnPoint[] itemSpawnPoints = FindObjectsOfType<ItemSpawnPoint>();
        foreach (ItemSpawnPoint spawnPoint in itemSpawnPoints)
        {
            spawnPoint.DeactivateAllItems();
        }
        Debug.Log($"Deactivated all quest items for failed quest: {id}");

        quest.ResetQuest();
        ChangeQuestState(quest.info.id, QuestState.CAN_START);
        Debug.Log($"Quest {id} reset to CAN_START state");
    }

    private void ClaimRewards(Quest quest)
    {
        Debug.Log($"ClaimRewards called for {quest.info.id}: Money={quest.info.moneyReward}, CommunityEXP={quest.info.experienceCommunityReward}");
        
        if (quest.info.moneyReward > 0)
        {
            GameEventsManager.instance.moneyEvents.MoneyGained(quest.info.moneyReward);
        }
        
        if (quest.info.experienceCommunityReward > 0)
        {
            GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceCommunityReward);
            Debug.Log($"ExperienceGained event fired with {quest.info.experienceCommunityReward} XP");
        }
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    private void OnApplicationQuit()
    {
        if (!saveQuestState) return;
        
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();

            if (questData.state == QuestState.IN_PROGRESS || questData.state == QuestState.CAN_FINISH)
            {
                Debug.Log($"Quest '{quest.info.id}' is in progress. Resetting to CAN_START before saving.");
                quest.ResetQuest();
                quest.state = QuestState.CAN_START;
                questData = quest.GetQuestData();
            }

            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try 
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }

    public void ResetAllQuests()
    {
        Debug.Log("Resetting all quests in the current area...");

        foreach (Quest quest in questMap.Values)
        {
            PlayerPrefs.DeleteKey(quest.info.id);
            
            if (quest.GetCurrentQuestStepPrefab() != null)
            {
                Destroy(quest.GetCurrentQuestStepPrefab().gameObject);
            }
        }

        PlayerPrefs.Save();

        questMap = CreateQuestMap();

        foreach (Quest quest in questMap.Values)
        {
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }
}
