using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverBoxQuestStep : QuestStep
{
    [Header("Delivery Settings")]
    [SerializeField] private string itemName = "DeliveryBox";
    [SerializeField] private float triggerSize = 5f;
    [SerializeField] private string deliveryPointName = "QuestPointF";
    
    [Header("Character Display")]
    [Tooltip("If true, will trigger character appearance at delivery point")]
    [SerializeField] private bool showCharacterOnDelivery = true;
    
    private bool hasDelivered = false;
    private ItemSpawnPoint itemSpawnPoint;
    private GameObject deliveryItem;

    private void Start()
    {
        Debug.Log("DeliverBoxQuestStep: Start() called");
        
        FindItemSpawnPoint();
        
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
            Debug.LogWarning("DeliverBoxQuestStep: deliveryPointName is not set! Using default position.");
            return;
        }

        GameObject deliveryPoint = GameObject.Find(deliveryPointName);
        if (deliveryPoint != null)
        {
            transform.position = deliveryPoint.transform.position;
            Debug.Log($"DeliverBoxQuestStep: Positioned at '{deliveryPointName}': {transform.position}");
        }
        else
        {
            Debug.LogWarning($"DeliverBoxQuestStep: Could not find delivery point '{deliveryPointName}'! Delivery zone at default position.");
        }
    }

    private void FindItemSpawnPoint()
    {
        itemSpawnPoint = FindObjectOfType<ItemSpawnPoint>();
        
        if (itemSpawnPoint != null)
        {
            deliveryItem = itemSpawnPoint.GetActiveItem(itemName);
            if (deliveryItem != null)
            {
                Debug.Log($"DeliverBoxQuestStep: Found active item '{itemName}': {deliveryItem.name}");
            }
            else
            {
                Debug.LogError($"No active item '{itemName}' found! The item should have been activated by PickupBoxQuestStep.");
            }
        }
        else
        {
            Debug.LogError("ItemSpawnPoint component not found in scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"DeliverBoxQuestStep: OnTriggerEnter with {other.name}, tag: {other.tag}, questId: {questId}");
        if (other.CompareTag("Player") && !hasDelivered)
        {
            if (!IsThisQuestActive())
            {
                Debug.Log($"DeliverBoxQuestStep: Player entered delivery zone, but quest '{questId}' is not active. Ignoring.");
                return;
            }

            Debug.Log($"DeliverBoxQuestStep: Player entered delivery zone for active quest '{questId}'!");
            
            StopQuestTimer();
            
            DeliverBox();
        }
    }

    private bool IsThisQuestActive()
    {
        Quest quest = QuestManager.instance.GetQuestById(questId);
        if (quest == null)
        {
            Debug.LogWarning($"DeliverBoxQuestStep: Quest '{questId}' not found!");
            return false;
        }

        bool isActive = quest.state == QuestState.IN_PROGRESS;
        Debug.Log($"DeliverBoxQuestStep: Quest '{questId}' state is {quest.state}, isActive: {isActive}");
        return isActive;
    }

    private void DeliverBox()
    {
        Debug.Log("DeliverBoxQuestStep: Delivering box!");
        hasDelivered = true;
        UpdateQuestStatus("Package delivered successfully!");
        
        Invoke(nameof(CompleteDelivery), 1.5f);
    }

    private void CompleteDelivery()
    {
        Debug.Log("DeliverBoxQuestStep: Package delivered to destination!");
        
        if (itemSpawnPoint != null && deliveryItem != null)
        {
            itemSpawnPoint.DeactivateItem(deliveryItem);
        }
        else
        {
            Debug.LogWarning("ItemSpawnPoint or deliveryItem is null - cannot deactivate item");
        }
        
        if (showCharacterOnDelivery)
        {
            ShowCharacterAtDeliveryPoint();
        }
        else
        {
            Debug.Log("DeliverBoxQuestStep: Character display disabled (showCharacterOnDelivery = false)");
        }
        
        UpdateQuestStatus("Delivered! Talk to the recipient to complete the quest.");
        FinishQuestStep();
    }

    private void ShowCharacterAtDeliveryPoint()
    {
        Debug.Log($"<color=yellow>DeliverBoxQuestStep: Searching for character controller at delivery point '{deliveryPointName}'</color>");
        
        QuestCharacterController[] characterControllers = FindObjectsOfType<QuestCharacterController>();
        Debug.Log($"DeliverBoxQuestStep: Found {characterControllers.Length} QuestCharacterController(s) in scene");
        
        foreach (QuestCharacterController controller in characterControllers)
        {
            GameObject controllerObject = controller.gameObject;
            string objectName = controllerObject.name;
            string parentName = controllerObject.transform.parent?.name ?? "null";
            
            Debug.Log($"DeliverBoxQuestStep: Checking controller on '{objectName}' (parent: '{parentName}')");
            
            if (controllerObject.name.Contains(deliveryPointName) || 
                controllerObject.transform.parent?.name.Contains(deliveryPointName) == true)
            {
                Debug.Log($"<color=green>DeliverBoxQuestStep: MATCH FOUND! Triggering character show at '{objectName}'</color>");
                controller.TriggerCharacterShow();
                Debug.Log($"DeliverBoxQuestStep: Triggered character show at {deliveryPointName}");
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

