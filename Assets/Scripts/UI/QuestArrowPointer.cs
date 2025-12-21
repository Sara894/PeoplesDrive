using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestArrowPointer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject arrowVisual;

    private Transform currentTarget;
    private Quest currentQuest;
    private bool isActive = false;
    private int currentStepIndex = 0;

    private void Start()
    {
        if (arrowVisual == null)
        {
            Transform visualChild = transform.Find("PointerArrow");
            if (visualChild != null)
            {
                arrowVisual = visualChild.gameObject;
            }
        }

        if (arrowVisual != null)
        {
            arrowVisual.SetActive(false);
        }

        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onAdvanceQuest += OnQuestAdvanced;
            GameEventsManager.instance.questEvents.onQuestStateChange += OnQuestStateChange;
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onAdvanceQuest -= OnQuestAdvanced;
            GameEventsManager.instance.questEvents.onQuestStateChange -= OnQuestStateChange;
        }
    }

    private void Update()
    {
        if (!isActive || currentTarget == null || arrowVisual == null)
            return;

        Vector3 pivotPosition = transform.position;
        Vector3 targetPosition = currentTarget.position;
        
        Vector3 directionToTarget = targetPosition - pivotPosition;
        directionToTarget.y = 0;
        
        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            
            float targetYAngle = targetRotation.eulerAngles.y;
            float currentYAngle = arrowVisual.transform.eulerAngles.y;
            
            float newYAngle = Mathf.LerpAngle(currentYAngle, targetYAngle, Time.deltaTime * rotationSpeed);
            
            arrowVisual.transform.eulerAngles = new Vector3(0, newYAngle, 0);
        }
    }

    public void ActivateArrowForQuest(Quest quest)
    {
        if (quest == null || quest.info == null)
        {
            Debug.LogWarning("QuestArrowPointer: Cannot activate - quest or quest info is null!");
            return;
        }

        if (quest.info.arrowWaypointNames == null || quest.info.arrowWaypointNames.Length == 0)
        {
            Debug.Log($"QuestArrowPointer: Quest '{quest.info.displayName}' has no arrow waypoints defined. Arrow will not activate.");
            return;
        }

        currentQuest = quest;
        currentStepIndex = quest.GetCurrentQuestStepIndex();
        
        Debug.Log($"QuestArrowPointer: Activating for quest '{quest.info.displayName}' at step {currentStepIndex}");
        
        SetTargetForCurrentStep();
    }

    public void DeactivateArrow()
    {
        isActive = false;
        currentQuest = null;
        currentTarget = null;
        currentStepIndex = 0;
        
        if (arrowVisual != null)
        {
            arrowVisual.SetActive(false);
        }
        
        Debug.Log("QuestArrowPointer: Deactivated");
    }

    private void SetTargetForCurrentStep()
    {
        if (currentQuest == null || currentQuest.info.arrowWaypointNames == null)
        {
            DeactivateArrow();
            return;
        }

        if (currentStepIndex >= currentQuest.info.arrowWaypointNames.Length)
        {
            Debug.Log($"QuestArrowPointer: No waypoint defined for step {currentStepIndex}. Deactivating.");
            DeactivateArrow();
            return;
        }

        string waypointName = currentQuest.info.arrowWaypointNames[currentStepIndex];

        if (string.IsNullOrEmpty(waypointName))
        {
            Debug.LogWarning($"QuestArrowPointer: Waypoint name at index {currentStepIndex} is empty!");
            DeactivateArrow();
            return;
        }

        GameObject waypointObject = FindWaypointByName(waypointName);
        if (waypointObject == null)
        {
            Debug.LogWarning($"QuestArrowPointer: Could not find GameObject named '{waypointName}' in scene!");
            DeactivateArrow();
            return;
        }

        currentTarget = waypointObject.transform;
        isActive = true;
        
        if (arrowVisual != null)
        {
            arrowVisual.SetActive(true);
        }
        
        Debug.Log($"QuestArrowPointer: Pointing to '{waypointObject.name}' (step {currentStepIndex}) for quest '{currentQuest.info.displayName}'");
    }

    private GameObject FindWaypointByName(string waypointName)
    {
        GameObject exactMatch = GameObject.Find(waypointName);
        if (exactMatch != null)
        {
            return exactMatch;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith(waypointName) && !obj.name.Contains("(Clone)"))
            {
                return obj;
            }
        }

        return null;
    }

    private void OnQuestAdvanced(string questId)
    {
        if (currentQuest == null || currentQuest.info.id != questId)
            return;

        currentStepIndex++;
        Debug.Log($"QuestArrowPointer: Quest '{questId}' advanced to step {currentStepIndex}");

        SetTargetForCurrentStep();
    }

    private void OnQuestStateChange(Quest quest)
    {
        if (quest.state == QuestState.IN_PROGRESS)
        {
            if (currentQuest == null)
            {
                Debug.Log($"QuestArrowPointer: Quest '{quest.info.displayName}' started directly (not via Quest Log). Activating arrow at current step.");
                ActivateArrowForQuest(quest);
            }
        }
        else if (currentQuest != null && quest.info.id == currentQuest.info.id)
        {
            if (quest.state == QuestState.FINISHED)
            {
                Debug.Log($"QuestArrowPointer: Quest '{quest.info.displayName}' finished. Deactivating arrow.");
                DeactivateArrow();
            }
            else if (quest.state == QuestState.CAN_START)
            {
                Debug.Log($"QuestArrowPointer: Quest '{quest.info.displayName}' returned to CAN_START (failed). Deactivating arrow.");
                DeactivateArrow();
            }
        }
    }
}
