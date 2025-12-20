using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [Header("Item Identification")]
    [SerializeField] private QuestItemType itemType = QuestItemType.None;

    public QuestItemType ItemType => itemType;
}
