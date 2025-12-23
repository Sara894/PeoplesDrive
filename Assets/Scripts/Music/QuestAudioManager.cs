using UnityEngine;

public class QuestAudioManager : MonoBehaviour
{
    public static QuestAudioManager instance { get; private set; }

    [Header("Quest Sound Effects")]
    [SerializeField] private AudioClip questCompleteSound;
    [SerializeField] private AudioClip questFailedSound;
    [SerializeField] private AudioClip checkpointSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float questSFXVolume = 1f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Quest Audio Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onFinishQuest += OnQuestFinished;
            GameEventsManager.instance.questEvents.onQuestFailed += OnQuestFailed;

            if (debugMode)
            {
                Debug.Log("QuestAudioManager: Subscribed to quest events");
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onFinishQuest -= OnQuestFinished;
            GameEventsManager.instance.questEvents.onQuestFailed -= OnQuestFailed;
        }
    }

    private void OnQuestFinished(string questId)
    {
        PlayQuestCompleteSound();

        if (debugMode)
        {
            Debug.Log($"QuestAudioManager: Quest '{questId}' completed - playing success sound");
        }
    }

    private void OnQuestFailed(string questId)
    {
        PlayQuestFailedSound();

        if (debugMode)
        {
            Debug.Log($"QuestAudioManager: Quest '{questId}' failed - playing failure sound");
        }
    }

    public void PlayQuestCompleteSound()
    {
        if (questCompleteSound != null)
        {
            AudioHelper.PlaySFX(questCompleteSound, questSFXVolume);
        }
        else if (debugMode)
        {
            Debug.LogWarning("QuestAudioManager: Quest complete sound is not assigned!");
        }
    }

    public void PlayQuestFailedSound()
    {
        if (questFailedSound != null)
        {
            AudioHelper.PlaySFX(questFailedSound, questSFXVolume);
        }
        else if (debugMode)
        {
            Debug.LogWarning("QuestAudioManager: Quest failed sound is not assigned!");
        }
    }

    public void PlayCheckpointSound()
    {
        if (checkpointSound != null)
        {
            AudioHelper.PlaySFX(checkpointSound, questSFXVolume);
        }
        else if (debugMode)
        {
            Debug.LogWarning("QuestAudioManager: Checkpoint sound is not assigned!");
        }
    }

    public void SetQuestCompleteSound(AudioClip clip)
    {
        questCompleteSound = clip;
    }

    public void SetQuestFailedSound(AudioClip clip)
    {
        questFailedSound = clip;
    }

    public void SetCheckpointSound(AudioClip clip)
    {
        checkpointSound = clip;
    }
}
