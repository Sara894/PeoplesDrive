using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [System.Serializable]
    public class QuestItemEntry
    {
        public string itemName;
        public GameObject itemObject;
    }

    [Header("Quest Items")]
    [SerializeField] private List<QuestItemEntry> questItems = new List<QuestItemEntry>();

    private HashSet<GameObject> itemsInUse = new HashSet<GameObject>();

    private void Awake()
    {
        foreach (var entry in questItems)
        {
            if (entry.itemObject != null)
            {
                entry.itemObject.SetActive(false);
            }
        }
    }

    public GameObject GetAvailableItem(string itemName)
    {
        foreach (var entry in questItems)
        {
            if (entry.itemName == itemName && entry.itemObject != null && !itemsInUse.Contains(entry.itemObject))
            {
                Debug.Log($"Found available item '{itemName}': {entry.itemObject.name}");
                return entry.itemObject;
            }
        }

        Debug.LogWarning($"No available item named '{itemName}' found! All instances may be in use or not set up.");
        return null;
    }

    public GameObject GetActiveItem(string itemName)
    {
        foreach (var entry in questItems)
        {
            if (entry.itemName == itemName && entry.itemObject != null && entry.itemObject.activeSelf)
            {
                Debug.Log($"Found active item '{itemName}': {entry.itemObject.name}");
                return entry.itemObject;
            }
        }

        Debug.LogWarning($"No active item named '{itemName}' found!");
        return null;
    }

    public void ActivateItem(GameObject itemObject)
    {
        if (itemObject != null)
        {
            itemObject.SetActive(true);
            itemsInUse.Add(itemObject);
            Debug.Log($"Activated item: {itemObject.name} (marked as IN USE)");
        }
        else
        {
            Debug.LogError("Cannot activate null item!");
        }
    }

    public void DeactivateItem(GameObject itemObject)
    {
        if (itemObject != null)
        {
            itemObject.SetActive(false);
            itemsInUse.Remove(itemObject);
            Debug.Log($"Deactivated item: {itemObject.name} (marked as AVAILABLE)");
        }
        else
        {
            Debug.LogError("Cannot deactivate null item!");
        }
    }

    public void DeactivateAllItems()
    {
        foreach (var entry in questItems)
        {
            if (entry.itemObject != null)
            {
                entry.itemObject.SetActive(false);
            }
        }
        itemsInUse.Clear();
        Debug.Log("Deactivated all items and cleared in-use tracking");
    }
}
