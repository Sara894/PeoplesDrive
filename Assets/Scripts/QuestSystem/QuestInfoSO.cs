using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {get; private set;}

    [Header("General")]
    public string displayName;

    [Header("Requierments")]
    public int levelRequiredToUnlock; //levelRequirement;

    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Arrow Waypoints")]
    [Tooltip("Optional: GameObject names for arrow to point to for each quest step. Example: 'QuestPointS', 'QuestPointF'. Leave empty if quest doesn't need arrow guidance.")]
    public string[] arrowWaypointNames;

    [Header("Timer")]
    [Tooltip("Optional: Time limit in seconds for this quest. Set to 0 for no time limit.")]
    public float timeLimitInSeconds = 0f;

    [Header("Rewards")]
    public int moneyReward;
    public int experienceCommunityReward;

    // id = name of Scriptable Object
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

}
