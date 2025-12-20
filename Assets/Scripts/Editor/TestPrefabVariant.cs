using UnityEngine;
using UnityEditor;

public class TestPrefabVariant : EditorWindow
{
    [MenuItem("Tools/Test Prefab Variant Values")]
    public static void TestPrefabValues()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Scripts/Resources/Quests/DeliveryQuestSimple/DeliverBoxQuestStep_Simple.prefab");
        
        if (prefab != null)
        {
            DeliverBoxQuestStep step = prefab.GetComponent<DeliverBoxQuestStep>();
            if (step != null)
            {
                SerializedObject so = new SerializedObject(step);
                SerializedProperty deliveryPointNameProp = so.FindProperty("deliveryPointName");
                
                Debug.Log($"<color=cyan>Prefab Variant Test:</color>");
                Debug.Log($"  Prefab: {prefab.name}");
                Debug.Log($"  DeliveryPointName: {deliveryPointNameProp.stringValue}");
                
                if (deliveryPointNameProp.stringValue == "QuestPointSimpleF")
                {
                    Debug.Log($"<color=green>✅ CORRECT! Variant has the right value!</color>");
                }
                else
                {
                    Debug.LogError($"❌ WRONG! Expected 'QuestPointSimpleF' but got '{deliveryPointNameProp.stringValue}'");
                }
            }
        }
        else
        {
            Debug.LogError("Prefab not found!");
        }
    }
}
