using System;
using UnityEngine;

public class AudioEvents
{
    public event Action<AudioClip> onPlayMusic;
    public void PlayMusic(AudioClip musicClip)
    {
        if (onPlayMusic != null)
        {
            onPlayMusic(musicClip);
        }
    }

    public event Action onStopMusic;
    public void StopMusic()
    {
        if (onStopMusic != null)
        {
            onStopMusic();
        }
    }

    public event Action<AudioClip, float> onPlaySoundEffect;
    public void PlaySoundEffect(AudioClip sfxClip, float volume = 1f)
    {
        if (onPlaySoundEffect != null)
        {
            onPlaySoundEffect(sfxClip, volume);
        }
    }

    public event Action<AudioClip, float> onPlayUISound;
    public void PlayUISound(AudioClip uiClip, float volume = 1f)
    {
        if (onPlayUISound != null)
        {
            onPlayUISound(uiClip, volume);
        }
    }

    public event Action<float> onSetMusicVolume;
    public void SetMusicVolume(float volume)
    {
        if (onSetMusicVolume != null)
        {
            onSetMusicVolume(volume);
        }
    }

    public event Action<float> onSetSFXVolume;
    public void SetSFXVolume(float volume)
    {
        if (onSetSFXVolume != null)
        {
            onSetSFXVolume(volume);
        }
    }

    public event Action<float> onSetUIVolume;
    public void SetUIVolume(float volume)
    {
        if (onSetUIVolume != null)
        {
            onSetUIVolume(volume);
        }
    }

    public event Action<float> onSetMasterVolume;
    public void SetMasterVolume(float volume)
    {
        if (onSetMasterVolume != null)
        {
            onSetMasterVolume(volume);
        }
    }
}
