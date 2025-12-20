using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Clip")]
public class SoundClipSO : ScriptableObject
{
    [Header("Sound Settings")]
    public string soundName;
    public AudioClip clip;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Header("Pitch Settings")]
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool randomizePitch = false;
    [Range(0f, 0.5f)]
    public float pitchVariation = 0.1f;
    
    [Header("Playback Settings")]
    public bool loop = false;

    public float GetRandomizedPitch()
    {
        if (randomizePitch)
        {
            return pitch + Random.Range(-pitchVariation, pitchVariation);
        }
        return pitch;
    }
}
