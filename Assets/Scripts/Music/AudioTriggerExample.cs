using UnityEngine;

public class AudioTriggerExample : MonoBehaviour
{
    [Header("Example Audio Clips")]
    [SerializeField] private AudioClip exampleMusicClip;
    [SerializeField] private AudioClip exampleSFXClip;
    [SerializeField] private AudioClip exampleUIClip;
    [SerializeField] private SoundClipSO exampleSoundClipSO;

    public void ExamplePlayMusic()
    {
        if (exampleMusicClip != null)
        {
            AudioHelper.PlayMusic(exampleMusicClip);
        }
    }

    public void ExamplePlaySFX()
    {
        if (exampleSFXClip != null)
        {
            AudioHelper.PlaySFX(exampleSFXClip);
        }
    }

    public void ExamplePlayUISound()
    {
        if (exampleUIClip != null)
        {
            AudioHelper.PlayUISound(exampleUIClip);
        }
    }

    public void ExamplePlaySoundClipSO()
    {
        if (exampleSoundClipSO != null)
        {
            AudioHelper.PlaySound(exampleSoundClipSO);
        }
    }

    public void ExampleStopMusic()
    {
        AudioHelper.StopMusic();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && exampleSFXClip != null)
        {
            AudioHelper.PlaySFX(exampleSFXClip);
        }
    }
}
