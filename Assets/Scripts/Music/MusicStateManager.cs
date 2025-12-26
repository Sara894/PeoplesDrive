using System.Collections;
using UnityEngine;

public class MusicStateManager : MonoBehaviour
{
    public static MusicStateManager instance { get; private set; }

    [Header("Music Tracks by State")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip freeRoamMusic;
    [SerializeField] private AudioClip questActiveMusic;

    [Header("Volume Control")]
    [Range(0f, 1f)]
    [SerializeField] private float backgroundMusicVolume = 1f;

    [Header("State Settings")]
    [SerializeField] private MusicState currentState = MusicState.Menu;
    [SerializeField] private bool debugMode = false;

    private MusicState previousState;
    private bool hasActiveQuest = false;

    public enum MusicState
    {
        Menu,
        FreeRoam,
        QuestActive
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Music State Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        SubscribeToEvents();
        StartCoroutine(PlayMusicAfterAudioManagerReady());
    }

    private void Update()
    {
        ApplyBackgroundMusicVolume();
    }

    private IEnumerator PlayMusicAfterAudioManagerReady()
    {
        while (AudioManager.instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.3f);
        
        PlayMusicForCurrentState();

        if (debugMode)
        {
            Debug.Log("MusicStateManager: Started playing initial music");
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onStartQuest += OnQuestStarted;
            GameEventsManager.instance.questEvents.onAdvanceQuest += OnQuestAdvanced;
            GameEventsManager.instance.questEvents.onFinishQuest += OnQuestFinished;
            GameEventsManager.instance.questEvents.onQuestFailed += OnQuestFailed;

            if (debugMode)
            {
                Debug.Log("MusicStateManager: Subscribed to quest events");
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onStartQuest -= OnQuestStarted;
            GameEventsManager.instance.questEvents.onAdvanceQuest -= OnQuestAdvanced;
            GameEventsManager.instance.questEvents.onFinishQuest -= OnQuestFinished;
            GameEventsManager.instance.questEvents.onQuestFailed -= OnQuestFailed;
        }
    }

    private void OnQuestStarted(string questId)
    {
        hasActiveQuest = true;
        SetMusicState(MusicState.QuestActive);

        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Quest '{questId}' started - switching to Quest music");
        }
    }

    private void OnQuestAdvanced(string questId)
    {
        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Quest '{questId}' advanced - maintaining Quest music");
        }
    }

    private void OnQuestFinished(string questId)
    {
        hasActiveQuest = false;
        SetMusicState(MusicState.FreeRoam);

        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Quest '{questId}' finished - switching to Free Roam music");
        }
    }

    private void OnQuestFailed(string questId)
    {
        hasActiveQuest = false;
        SetMusicState(MusicState.FreeRoam);

        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Quest '{questId}' failed - switching to Free Roam music");
        }
    }

    public void SetMusicState(MusicState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        previousState = currentState;
        currentState = newState;

        PlayMusicForCurrentState();

        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Music state changed from {previousState} to {currentState}");
        }
    }

    private void PlayMusicForCurrentState()
    {
        AudioClip musicToPlay = null;

        switch (currentState)
        {
            case MusicState.Menu:
                musicToPlay = menuMusic;
                break;
            case MusicState.FreeRoam:
                musicToPlay = freeRoamMusic;
                break;
            case MusicState.QuestActive:
                musicToPlay = questActiveMusic;
                break;
        }

        if (musicToPlay != null)
        {
            AudioHelper.PlayMusic(musicToPlay);

            if (debugMode)
            {
                Debug.Log($"MusicStateManager: Now playing '{musicToPlay.name}' for state {currentState}");
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"MusicStateManager: No music assigned for state {currentState}");
            }
        }
    }

    private void ApplyBackgroundMusicVolume()
    {
        if (AudioManager.instance != null)
        {
            AudioSource musicSource = AudioManager.instance.GetMusicAudioSource();
            if (musicSource != null && musicSource.volume != backgroundMusicVolume)
            {
                musicSource.volume = backgroundMusicVolume;
            }
        }
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = Mathf.Clamp01(volume);
        ApplyBackgroundMusicVolume();

        if (debugMode)
        {
            Debug.Log($"MusicStateManager: Background music volume set to {backgroundMusicVolume:F2}");
        }
    }

    public float GetBackgroundMusicVolume()
    {
        return backgroundMusicVolume;
    }

    public void TransitionToMenu()
    {
        SetMusicState(MusicState.Menu);
    }

    public void TransitionToFreeRoam()
    {
        SetMusicState(MusicState.FreeRoam);
    }

    public void TransitionToQuestActive()
    {
        SetMusicState(MusicState.QuestActive);
    }

    public MusicState GetCurrentState()
    {
        return currentState;
    }

    public bool HasActiveQuest()
    {
        return hasActiveQuest;
    }

    public void SetMenuMusic(AudioClip clip)
    {
        menuMusic = clip;
        if (currentState == MusicState.Menu)
        {
            PlayMusicForCurrentState();
        }
    }

    public void SetFreeRoamMusic(AudioClip clip)
    {
        freeRoamMusic = clip;
        if (currentState == MusicState.FreeRoam)
        {
            PlayMusicForCurrentState();
        }
    }

    public void SetQuestActiveMusic(AudioClip clip)
    {
        questActiveMusic = clip;
        if (currentState == MusicState.QuestActive)
        {
            PlayMusicForCurrentState();
        }
    }
}
