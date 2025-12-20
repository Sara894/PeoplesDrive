using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommunityLevelManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int startingExperience = 25;

    public int currentLevel { get; private set; }
    public int currentExperience { get; private set; }

    private void Awake()
    {
        currentLevel = startingLevel;
        currentExperience = startingExperience;
    }

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            Debug.Log("CommunityLevelManager: Subscribing to events in Start()");
            GameEventsManager.instance.playerEvents.onExperienceGained += ExperienceGained;
            
            GameEventsManager.instance.playerEvents.CommunityLevelChange(currentLevel);
            GameEventsManager.instance.playerEvents.CommunityExperienceChange(currentExperience);
        }
        else
        {
            Debug.LogError("CommunityLevelManager: GameEventsManager.instance is NULL at Start!");
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onExperienceGained -= ExperienceGained;
        }
    }

    private void ExperienceGained(int experience)
    {
        Debug.Log($"CommunityLevelManager.ExperienceGained called with {experience} XP");
        
        currentExperience += experience;
        GameEventsManager.instance.playerEvents.CommunityExperienceChange(currentExperience);

        Debug.Log($"Current Community XP is now: {currentExperience}/100");

        if (currentExperience >= GlobalConstants.experienceToLevelUpCommunity)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExperience -= GlobalConstants.experienceToLevelUpCommunity;
        currentLevel++;
        
        GameEventsManager.instance.playerEvents.CommunityLevelChange(currentLevel);
        GameEventsManager.instance.playerEvents.CommunityExperienceChange(currentExperience);
        
        Debug.Log($"Community Level Up! Now level {currentLevel} - New area unlocked!");
    }

    private void ResetArea()
    {
        Debug.LogError("Community XP dropped below 0! Resetting area...");
        
        currentExperience = startingExperience;
        
        GameEventsManager.instance.playerEvents.CommunityExperienceChange(currentExperience);
        
        RespawnPlayer();
        
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.ResetAllQuests();
        }
        
        StartCoroutine(ReloadCurrentScene());
    }
    
    private void RespawnPlayer()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner != null)
        {
            spawner.RespawnPlayer();
        }
        else
        {
            Debug.LogWarning("CommunityLevelManager: PlayerSpawner not found - skipping respawn");
        }
    }

    private IEnumerator ReloadCurrentScene()
    {
        yield return new WaitForSeconds(2f);
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
