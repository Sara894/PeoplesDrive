using UnityEditor;
using UnityEngine;

public class EnableBothInputSystems
{
    [MenuItem("Tools/Enable Both Input Systems")]
    public static void EnableBoth()
    {
        var playerSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        var activeInputHandler = playerSettings.FindProperty("activeInputHandler");
        
        if (activeInputHandler != null)
        {
            activeInputHandler.intValue = 2;
            playerSettings.ApplyModifiedProperties();
            Debug.Log("<color=green>Successfully enabled BOTH Input Systems! Please restart Unity Editor for changes to take effect.</color>");
        }
        else
        {
            Debug.LogError("Could not find activeInputHandler property in Player Settings!");
        }
    }
}
