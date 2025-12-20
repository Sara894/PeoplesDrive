using System.Collections;
using UnityEngine;

public class PickupBoxQuestStep_Simple : QuestStep
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "DeliveryBox";
    [SerializeField] private float autoPickupDelay = 0.5f;
    
    private GameObject deliveryItem;
    private ItemSpawnPoint itemSpawnPoint;
    private Movement playerMovement;

    private void Start()
    {
        Debug.Log("PickupBoxQuestStep_Simple: Start() called - Auto-pickup enabled");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.StopMovement();
                Debug.Log("PickupBoxQuestStep_Simple: Player movement STOPPED for pickup");
            }
        }
        
        FindItemSpawnPoint();
        UpdateQuestStatus("Preparing package for pickup...");
        
        Invoke(nameof(PickupBox), autoPickupDelay);
    }

    private void FindItemSpawnPoint()
    {
        Debug.Log("PickupBoxQuestStep_Simple: Finding ItemSpawnPoint...");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            itemSpawnPoint = player.GetComponentInChildren<ItemSpawnPoint>();
            
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
                Debug.LogError("ItemSpawnPoint component not found on Player or children!");
            }
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    private void PickupBox()
    {
        Debug.Log($"PickupBoxQuestStep_Simple: Auto-picking up '{itemName}'!");

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
        
        if (playerMovement != null)
        {
            playerMovement.ResumeMovement();
            Debug.Log("PickupBoxQuestStep_Simple: Player movement RESUMED after pickup");
        }
        
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
