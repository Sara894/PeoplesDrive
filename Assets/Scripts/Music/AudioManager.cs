using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Volume Settings")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float uiVolume = 1f;

    [Header("Music Settings")]
    [SerializeField] private bool fadeMusic = true;
    [SerializeField] private float musicFadeDuration = 1f;

    private Coroutine musicFadeCoroutine;
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string UI_VOLUME_KEY = "UIVolume";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
        LoadVolumeSettings();
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (uiSource == null)
        {
            GameObject uiObj = new GameObject("UISource");
            uiObj.transform.SetParent(transform);
            uiSource = uiObj.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.playOnAwake = false;
        }

        ApplyVolumeSettings();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.onPlayMusic += PlayMusic;
            GameEventsManager.instance.audioEvents.onStopMusic += StopMusic;
            GameEventsManager.instance.audioEvents.onPlaySoundEffect += PlaySoundEffect;
            GameEventsManager.instance.audioEvents.onPlayUISound += PlayUISound;
            GameEventsManager.instance.audioEvents.onSetMusicVolume += SetMusicVolume;
            GameEventsManager.instance.audioEvents.onSetSFXVolume += SetSFXVolume;
            GameEventsManager.instance.audioEvents.onSetUIVolume += SetUIVolume;
            GameEventsManager.instance.audioEvents.onSetMasterVolume += SetMasterVolume;
            Debug.Log("AudioManager: Subscribed to audio events");
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.onPlayMusic -= PlayMusic;
            GameEventsManager.instance.audioEvents.onStopMusic -= StopMusic;
            GameEventsManager.instance.audioEvents.onPlaySoundEffect -= PlaySoundEffect;
            GameEventsManager.instance.audioEvents.onPlayUISound -= PlayUISound;
            GameEventsManager.instance.audioEvents.onSetMusicVolume -= SetMusicVolume;
            GameEventsManager.instance.audioEvents.onSetSFXVolume -= SetSFXVolume;
            GameEventsManager.instance.audioEvents.onSetUIVolume -= SetUIVolume;
            GameEventsManager.instance.audioEvents.onSetMasterVolume -= SetMasterVolume;
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play null music clip");
            return;
        }

        if (musicSource.clip == musicClip && musicSource.isPlaying)
        {
            return;
        }

        if (fadeMusic && musicSource.isPlaying)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
            musicFadeCoroutine = StartCoroutine(FadeMusic(musicClip));
        }
        else
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        Debug.Log($"AudioManager: Playing music '{musicClip.name}'");
    }

    public void StopMusic()
    {
        if (fadeMusic && musicSource.isPlaying)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
            musicFadeCoroutine = StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }

        Debug.Log("AudioManager: Music stopped");
    }

    public void PlaySoundEffect(AudioClip sfxClip, float volume = 1f)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play null SFX clip");
            return;
        }

        sfxSource.PlayOneShot(sfxClip, volume);
    }

    public void PlayUISound(AudioClip uiClip, float volume = 1f)
    {
        if (uiClip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play null UI clip");
            return;
        }

        uiSource.PlayOneShot(uiClip, volume);
    }

    public void PlaySound(SoundClipSO soundClip)
    {
        if (soundClip == null || soundClip.clip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play null SoundClipSO");
            return;
        }

        AudioSource sourceToUse = sfxSource;
        sourceToUse.pitch = soundClip.GetRandomizedPitch();
        sourceToUse.PlayOneShot(soundClip.clip, soundClip.volume);
        sourceToUse.pitch = 1f;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
        Debug.Log($"AudioManager: Master volume set to {masterVolume:F2}");
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
        Debug.Log($"AudioManager: Music volume set to {musicVolume:F2}");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
        Debug.Log($"AudioManager: SFX volume set to {sfxVolume:F2}");
    }

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
        Debug.Log($"AudioManager: UI volume set to {uiVolume:F2}");
    }

    private void ApplyVolumeSettings()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
        
        if (uiSource != null)
            uiSource.volume = uiVolume * masterVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.SetFloat(UI_VOLUME_KEY, uiVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, 1f);

        ApplyVolumeSettings();
        Debug.Log($"AudioManager: Loaded volume settings - Master: {masterVolume:F2}, Music: {musicVolume:F2}, SFX: {sfxVolume:F2}, UI: {uiVolume:F2}");
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < musicFadeDuration / 2f)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / (musicFadeDuration / 2f));
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        timer = 0f;
        while (timer < musicFadeDuration / 2f)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume * masterVolume, timer / (musicFadeDuration / 2f));
            yield return null;
        }

        musicSource.volume = musicVolume * masterVolume;
        musicFadeCoroutine = null;
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume * masterVolume;
        musicFadeCoroutine = null;
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetUIVolume() => uiVolume;
    public bool IsMusicPlaying() => musicSource != null && musicSource.isPlaying;
    public AudioClip GetCurrentMusic() => musicSource != null ? musicSource.clip : null;
}
