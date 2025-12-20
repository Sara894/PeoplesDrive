using UnityEngine;
using TMPro;

public class CommunityLevelDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Display Settings")]
    [SerializeField] private string prefix = "Level ";
    [SerializeField] private bool showPrefixInText = true;

    private void Start()
    {
        Debug.Log("CommunityLevelDisplay: Start - Subscribing to onCommunityLevelChange");
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onCommunityLevelChange += UpdateLevelDisplay;
            Debug.Log("CommunityLevelDisplay: Successfully subscribed to onCommunityLevelChange");
        }
        else
        {
            Debug.LogError("CommunityLevelDisplay: GameEventsManager.instance is NULL in Start!");
        }

        if (levelText == null)
        {
            levelText = GetComponent<TextMeshProUGUI>();
        }

        if (levelText == null)
        {
            Debug.LogError("CommunityLevelDisplay: No TextMeshProUGUI component found!");
        }

        CommunityLevelManager levelManager = FindObjectOfType<CommunityLevelManager>();
        if (levelManager != null)
        {
            UpdateLevelDisplay(levelManager.currentLevel);
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onCommunityLevelChange -= UpdateLevelDisplay;
        }
    }

    private void UpdateLevelDisplay(int level)
    {
        Debug.Log($"CommunityLevelDisplay: Updating level display to {level}");
        
        if (levelText != null)
        {
            if (showPrefixInText)
            {
                levelText.text = $"{prefix}{level}";
            }
            else
            {
                levelText.text = level.ToString();
            }
        }
        else
        {
            Debug.LogError("CommunityLevelDisplay: levelText is null!");
        }
    }
}
