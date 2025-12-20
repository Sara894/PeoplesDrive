using UnityEngine;
using UnityEngine.UI;

public class CommunityEXPManager : MonoBehaviour
{
    private Image expFillImage;

    private void Start()
    {
        expFillImage = GetComponent<Image>();
        
        if (expFillImage == null)
        {
            Debug.LogError("CommunityEXPManager: No Image component found! Make sure this script is on a GameObject with an Image component.");
            return;
        }

        if (GameEventsManager.instance != null)
        {
            Debug.Log("CommunityEXPManager: Subscribing to onCommunityExperienceChange event");
            GameEventsManager.instance.playerEvents.onCommunityExperienceChange += UpdateExpBar;
        }
        else
        {
            Debug.LogError("CommunityEXPManager: GameEventsManager.instance is NULL at Start!");
        }

        InitializeExpBar();
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onCommunityExperienceChange -= UpdateExpBar;
        }
    }

    private void InitializeExpBar()
    {
        CommunityLevelManager levelManager = FindObjectOfType<CommunityLevelManager>();
        
        if (levelManager != null)
        {
            UpdateExpBar(levelManager.currentExperience);
        }
        else
        {
            Debug.LogWarning("CommunityEXPManager: Could not find CommunityLevelManager!");
        }
    }

    private void UpdateExpBar(int currentExperience)
    {
        if (expFillImage != null)
        {
            float fillAmount = Mathf.Clamp01(currentExperience / 100f);
            expFillImage.fillAmount = fillAmount;
            
            Debug.Log($"Community EXP Bar updated: {currentExperience}/100 (Fill: {fillAmount:F2})");
        }
    }
}
