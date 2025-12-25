using UnityEngine;

public static class SaveManager
{
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All progress reset");
    }
}
