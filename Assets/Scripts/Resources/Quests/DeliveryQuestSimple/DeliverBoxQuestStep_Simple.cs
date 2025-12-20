using UnityEngine;

public class DeliverBoxQuestStep_Simple : QuestStep
{
    [Header("Delivery Settings")]
    [SerializeField] private string itemName = "DeliveryBox";
    [SerializeField] private float triggerSize = 5f;
    [SerializeField] private string deliveryPointName = "QuestPointSimpleF";
    
    [Header("Character Display")]
    [Tooltip("If true, will trigger character appearance at delivery point")]
    [SerializeField] private bool showCharacterOnDelivery = true;
    
    private bool hasDelivered = false;
    private ItemSpawnPoint itemSpawnPoint;
    private GameObject deliveryItem;
    private Movement playerMovement;

    private void Start()
    {
        Debug.Log("DeliverBoxQuestStep_Simple: Start() called");
        
        FindItemSpawnPoint();
        FindPlayerMovement();
        
        BoxCollider trigger = GetComponent<BoxCollider>();
        if (trigger != null)
        {
            trigger.size = new Vector3(triggerSize, triggerSize * 0.5f, triggerSize);
            Debug.Log($"Delivery zone trigger size set to: {trigger.size}");
        }
        
        PositionAtDeliveryPoint();
        
        UpdateQuestStatus();
    }

    private void PositionAtDeliveryPoint()
    {
        if (string.IsNullOrEmpty(deliveryPointName))
        {
            Debug.LogWarning("DeliverBoxQuestStep_Simple: deliveryPointName is not set! Using default position.");
            return;
        }

        GameObject deliveryPoint = GameObject.Find(deliveryPointName);
        if (deliveryPoint != null)
        {
            transform.position = deliveryPoint.transform.position;
            Debug.Log($"DeliverBoxQuestStep_Simple: Positioned at '{deliveryPointName}': {transform.position}");
        }
        else
        {
            Debug.LogWarning($"DeliverBoxQuestStep_Simple: Could not find delivery point '{deliveryPointName}'! Delivery zone at default position.");
        }
    }

    private void FindItemSpawnPoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            itemSpawnPoint = player.GetComponentInChildren<ItemSpawnPoint>();
            
            if (itemSpawnPoint != null)
            {
                deliveryItem = itemSpawnPoint.GetActiveItem(itemName);
                if (deliveryItem != null)
                {
                    Debug.Log($"DeliverBoxQuestStep_Simple: Found active item '{itemName}': {deliveryItem.name}");
                }
                else
                {
                    Debug.LogError($"No active item '{itemName}' found! The item should have been activated by PickupBoxQuestStep_Simple.");
                }
            }
            else
            {
                Debug.LogError("ItemSpawnPoint not found on Player!");
            }
        }
    }

    private void FindPlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<Movement>();
            if (playerMovement != null)
            {
                Debug.Log("DeliverBoxQuestStep_Simple: Found Movement component on player");
            }
            else
            {
                Debug.LogError("Movement component not found on Player!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"DeliverBoxQuestStep_Simple: OnTriggerEnter with {other.name}, tag: {other.tag}, questId: {questId}");
        if (other.CompareTag("Player") && !hasDelivered)
        {
            if (!IsThisQuestActive())
            {
                Debug.Log($"DeliverBoxQuestStep_Simple: Player entered delivery zone, but quest '{questId}' is not active. Ignoring.");
                return;
            }

            Debug.Log($"DeliverBoxQuestStep_Simple: Player entered delivery zone for active quest '{questId}'!");
            
            if (playerMovement != null)
            {
                playerMovement.StopMovement();
            }
            
            DeliverBox();
        }
    }

    private bool IsThisQuestActive()
    {
        Quest quest = QuestManager.instance.GetQuestById(questId);
        if (quest == null)
        {
            Debug.LogWarning($"DeliverBoxQuestStep_Simple: Quest '{questId}' not found!");
            return false;
        }

        bool isActive = quest.state == QuestState.IN_PROGRESS;
        Debug.Log($"DeliverBoxQuestStep_Simple: Quest '{questId}' state is {quest.state}, isActive: {isActive}");
        return isActive;
    }

    private void DeliverBox()
    {
        Debug.Log("DeliverBoxQuestStep_Simple: Delivering box!");
        hasDelivered = true;
        UpdateQuestStatus("Package delivered successfully!");
        
        Invoke(nameof(CompleteDelivery), 1.5f);
    }

    private void CompleteDelivery()
    {
        Debug.Log("DeliverBoxQuestStep_Simple: Package delivered to destination!");
        
        if (itemSpawnPoint != null && deliveryItem != null)
        {
            itemSpawnPoint.DeactivateItem(deliveryItem);
        }
        else
        {
            Debug.LogWarning("ItemSpawnPoint or deliveryItem is null - cannot deactivate item");
        }
        
        if (playerMovement != null)
        {
            playerMovement.ResumeMovement();
        }
        
        if (showCharacterOnDelivery)
        {
            ShowCharacterAtDeliveryPoint();
        }
        else
        {
            Debug.Log("DeliverBoxQuestStep_Simple: Character display disabled (showCharacterOnDelivery = false)");
        }
        
        UpdateQuestStatus("Delivered! Talk to the recipient to complete the quest.");
        FinishQuestStep();
    }

    private void ShowCharacterAtDeliveryPoint()
    {
        Debug.Log($"<color=yellow>DeliverBoxQuestStep_Simple: Searching for character controller at delivery point '{deliveryPointName}'</color>");
        
        QuestCharacterController[] characterControllers = FindObjectsOfType<QuestCharacterController>();
        Debug.Log($"DeliverBoxQuestStep_Simple: Found {characterControllers.Length} QuestCharacterController(s) in scene");
        
        foreach (QuestCharacterController controller in characterControllers)
        {
            GameObject controllerObject = controller.gameObject;
            string objectName = controllerObject.name;
            string parentName = controllerObject.transform.parent?.name ?? "null";
            
            Debug.Log($"DeliverBoxQuestStep_Simple: Checking controller on '{objectName}' (parent: '{parentName}')");
            
            if (controllerObject.name.Contains(deliveryPointName) || 
                controllerObject.transform.parent?.name.Contains(deliveryPointName) == true)
            {
                Debug.Log($"<color=green>DeliverBoxQuestStep_Simple: MATCH FOUND! Triggering character show at '{objectName}'</color>");
                controller.TriggerCharacterShow();
                Debug.Log($"DeliverBoxQuestStep_Simple: Triggered character show at {deliveryPointName}");
                break;
            }
        }
    }

    private void UpdateQuestStatus(string customMessage = null)
    {
        string status = customMessage ?? "Drive to the destination and deliver the package.";
        ChangeState("", status);
    }

    protected override void SetQuestStepState(string state)
    {
    }
}
