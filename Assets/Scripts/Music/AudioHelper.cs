using UnityEngine;

public static class AudioHelper
{
    public static void PlayMusic(AudioClip musicClip)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.PlayMusic(musicClip);
        }
    }

    public static void StopMusic()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.StopMusic();
        }
    }

    public static void PlaySFX(AudioClip sfxClip, float volume = 1f)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.PlaySoundEffect(sfxClip, volume);
        }
    }

    public static void PlayUISound(AudioClip uiClip, float volume = 1f)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.PlayUISound(uiClip, volume);
        }
    }

    public static void PlaySound(SoundClipSO soundClip)
    {
        if (AudioManager.instance != null && soundClip != null)
        {
            AudioManager.instance.PlaySound(soundClip);
        }
    }

    public static void SetMasterVolume(float volume)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.SetMasterVolume(volume);
        }
    }

    public static void SetMusicVolume(float volume)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.SetMusicVolume(volume);
        }
    }

    public static void SetSFXVolume(float volume)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.SetSFXVolume(volume);
        }
    }

    public static void SetUIVolume(float volume)
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.audioEvents.SetUIVolume(volume);
        }
    }
}
