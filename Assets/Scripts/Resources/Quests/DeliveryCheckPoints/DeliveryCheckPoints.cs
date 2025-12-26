using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCheckPoints : QuestStep
{
    [Header("Checkpoint Config")]
    [SerializeField] private string checkpointNumberString = "first";
    
    [Header("Item Requirement")]
    [SerializeField] private bool requiresDeliveryItem = true;
    [SerializeField] private string requiredItemName = "DeliveryBox";
    
    [Header("Time Bonus")]
    [SerializeField] private float timeBonusSeconds = 2f;
    
    private BoxCollider triggerCollider;
    private bool checkpointReached = false;
    private GameObject player;
    private ItemSpawnPoint itemSpawnPoint;

    private void OnEnable()
    {
        checkpointReached = false;
        Debug.Log($"DeliveryCheckPoints: {checkpointNumberString} checkpoint enabled and reset");
    }

    private void Start()
    {
        SetupTriggerZone();
        FindPlayer();
        UpdateQuestStatus($"Deliver to the {checkpointNumberString} checkpoint.");
    }

    private void SetupTriggerZone()
    {
        triggerCollider = gameObject.GetComponent<BoxCollider>();
        if (triggerCollider == null)
        {
            Debug.LogError($"DeliveryCheckPoints: No BoxCollider found on {gameObject.name}! Add one in the prefab.");
            return;
        }
        
        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.LogWarning($"DeliveryCheckPoints: BoxCollider on {gameObject.name} was not set to trigger. Fixed automatically.");
        }
        
        Debug.Log($"DeliveryCheckPoints: Trigger zone setup for {checkpointNumberString} checkpoint");
    }
    
    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (requiresDeliveryItem)
        {
            itemSpawnPoint = FindObjectOfType<ItemSpawnPoint>();
            if (itemSpawnPoint == null)
            {
                Debug.LogWarning("DeliveryCheckPoints: ItemSpawnPoint not found in scene!");
            }
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (checkpointReached) return;
        
        if (otherCollider.CompareTag("Player"))
        {
            if (requiresDeliveryItem && !HasRequiredItem())
            {
                UpdateQuestStatus($"You need the {requiredItemName} to complete this checkpoint!");
                Debug.LogWarning($"DeliveryCheckPoints: Player reached {checkpointNumberString} checkpoint without required item!");
                return;
            }
            
            ReachCheckpoint();
        }
    }
    
    private bool HasRequiredItem()
    {
        if (itemSpawnPoint == null) return false;
        
        GameObject activeItem = itemSpawnPoint.GetActiveItem(requiredItemName);
        return activeItem != null && activeItem.activeSelf;
    }
    
    private void ReachCheckpoint()
    {
        Debug.Log($"DeliveryCheckPoints: ReachCheckpoint() START for {checkpointNumberString}");
        checkpointReached = true;
        
        Debug.Log($"DeliveryCheckPoints: Calling AddTimeBonusToQuest()");
        AddTimeBonusToQuest();
        
        Debug.Log($"DeliveryCheckPoints: Calling PlayCheckpointSound()");
        PlayCheckpointSound();
        
        Debug.Log($"DeliveryCheckPoints: Calling UpdateQuestStatus()");
        UpdateQuestStatus($"✓ {checkpointNumberString} checkpoint reached! +{timeBonusSeconds}s");
        
        Debug.Log($"DeliveryCheckPoints: {checkpointNumberString} checkpoint completed!");
        
        Debug.Log($"DeliveryCheckPoints: Calling FinishQuestStep() for questId={questId}");
        FinishQuestStep();
        
        Debug.Log($"DeliveryCheckPoints: ReachCheckpoint() END");
    }
    
    private void PlayCheckpointSound()
    {
        if (QuestAudioManager.instance != null)
        {
            QuestAudioManager.instance.PlayCheckpointSound();
        }
    }
    
    private void AddTimeBonusToQuest()
    {
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.AddBonusTime(timeBonusSeconds);
            Debug.Log($"DeliveryCheckPoints: Added {timeBonusSeconds}s bonus time for reaching {checkpointNumberString} checkpoint");
        }
        else
        {
            Debug.LogWarning("DeliveryCheckPoints: Timer not found - cannot add bonus time!");
        }
    }
    
    private void UpdateQuestStatus(string status)
    {
        Debug.Log($"DeliveryCheckPoints: UpdateQuestStatus called with status='{status}'");
        try
        {
            ChangeState("", status);
            Debug.Log($"DeliveryCheckPoints: ChangeState completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DeliveryCheckPoints: ChangeState threw exception: {e.Message}\n{e.StackTrace}");
        }
    }

    protected override void SetQuestStepState(string state)
    {
    }
}
