using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Scene Music Configuration")]
    [SerializeField] private bool overrideMenuMusic = false;
    [SerializeField] private AudioClip sceneMenuMusic;
    
    [SerializeField] private bool overrideFreeRoamMusic = false;
    [SerializeField] private AudioClip sceneFreeRoamMusic;
    
    [SerializeField] private bool overrideQuestMusic = false;
    [SerializeField] private AudioClip sceneQuestMusic;

    [Header("Auto Transition")]
    [SerializeField] private bool transitionOnSceneLoad = true;
    [SerializeField] private MusicStateManager.MusicState initialState = MusicStateManager.MusicState.FreeRoam;

    private void Start()
    {
        ApplySceneMusicOverrides();

        if (transitionOnSceneLoad && MusicStateManager.instance != null)
        {
            MusicStateManager.instance.SetMusicState(initialState);
        }
    }

    private void ApplySceneMusicOverrides()
    {
        if (MusicStateManager.instance == null)
        {
            Debug.LogWarning("SceneMusicTrigger: MusicStateManager not found in scene!");
            return;
        }

        if (overrideMenuMusic && sceneMenuMusic != null)
        {
            MusicStateManager.instance.SetMenuMusic(sceneMenuMusic);
            Debug.Log($"SceneMusicTrigger: Menu music overridden with '{sceneMenuMusic.name}'");
        }

        if (overrideFreeRoamMusic && sceneFreeRoamMusic != null)
        {
            MusicStateManager.instance.SetFreeRoamMusic(sceneFreeRoamMusic);
            Debug.Log($"SceneMusicTrigger: Free Roam music overridden with '{sceneFreeRoamMusic.name}'");
        }

        if (overrideQuestMusic && sceneQuestMusic != null)
        {
            MusicStateManager.instance.SetQuestActiveMusic(sceneQuestMusic);
            Debug.Log($"SceneMusicTrigger: Quest music overridden with '{sceneQuestMusic.name}'");
        }
    }
}
