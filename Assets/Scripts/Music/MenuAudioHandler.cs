using UnityEngine;
using UnityEngine.UI;

public class MenuAudioHandler : MonoBehaviour
{
    [Header("Menu Sound")]
    [SerializeField] private AudioClip menuClickSound;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;

    [Header("Auto-Setup")]
    [SerializeField] private bool autoAttachToButtons = true;

    private void Start()
    {
        if (autoAttachToButtons)
        {
            AttachClickSoundToAllButtons();
        }
    }

    private void AttachClickSoundToAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        
        foreach (Button button in buttons)
        {
            if (button.onClick.GetPersistentEventCount() == 0 || 
                !HasMenuClickListener(button))
            {
                button.onClick.AddListener(() => PlayMenuClickSound());
            }
        }

        Debug.Log($"MenuAudioHandler: Attached click sounds to {buttons.Length} buttons");
    }

    private bool HasMenuClickListener(Button button)
    {
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            if (button.onClick.GetPersistentMethodName(i) == "PlayMenuClickSound")
            {
                return true;
            }
        }
        return false;
    }

    public void PlayMenuClickSound()
    {
        if (menuClickSound != null)
        {
            AudioHelper.PlayUISound(menuClickSound, clickVolume);
        }
    }

    public void SetMenuClickSound(AudioClip clip)
    {
        menuClickSound = clip;
    }

    public static void PlayClick()
    {
        MenuAudioHandler handler = FindObjectOfType<MenuAudioHandler>();
        if (handler != null)
        {
            handler.PlayMenuClickSound();
        }
    }
}
