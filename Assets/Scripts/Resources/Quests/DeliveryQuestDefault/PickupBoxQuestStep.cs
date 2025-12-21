using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBoxQuestStep : QuestStep
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "DeliveryBox";
    [SerializeField] private float autoPickupDelay = 0.5f;
    
    private GameObject deliveryItem;
    private ItemSpawnPoint itemSpawnPoint;

    private void Start()
    {
        Debug.Log("PickupBoxQuestStep: Start() called - Auto-pickup enabled");
        
        FindItemSpawnPoint();
        UpdateQuestStatus("Preparing package for pickup...");
        
        Invoke(nameof(PickupBox), autoPickupDelay);
    }

    private void FindItemSpawnPoint()
    {
        Debug.Log("PickupBoxQuestStep: Finding ItemSpawnPoint...");
        
        itemSpawnPoint = FindObjectOfType<ItemSpawnPoint>();
        
        if (itemSpawnPoint != null)
        {
            Debug.Log($"ItemSpawnPoint found on {itemSpawnPoint.gameObject.name}");
            
            deliveryItem = itemSpawnPoint.GetAvailableItem(itemName);
            if (deliveryItem != null)
            {
                Debug.Log($"Found available item '{itemName}': {deliveryItem.name}, currently active: {deliveryItem.activeSelf}");
            }
            else
            {
                Debug.LogError($"No available item '{itemName}' found! Check ItemSpawnPoint setup.");
            }
        }
        else
        {
            Debug.LogError("ItemSpawnPoint component not found in scene!");
        }
    }

    private void PickupBox()
    {
        Debug.Log($"PickupBoxQuestStep: Auto-picking up '{itemName}'!");

        if (itemSpawnPoint != null && deliveryItem != null)
        {
            itemSpawnPoint.ActivateItem(deliveryItem);
            Debug.Log($"=== ITEM ACTIVATED === {deliveryItem.name}, Active: {deliveryItem.activeSelf}");
        }
        else
        {
            Debug.LogError("Cannot activate item - itemSpawnPoint or deliveryItem is null!");
        }

        UpdateQuestStatus("Package loaded! Deliver it to the destination.");
        
        StartQuestTimer();
        
        FinishQuestStep();
    }

    private void UpdateQuestStatus(string status)
    {
        ChangeState("", status);
    }

    protected override void SetQuestStepState(string state)
    {
    }
}
