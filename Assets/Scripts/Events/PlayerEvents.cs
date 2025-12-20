using System;

public class PlayerEvents
{
    public event Action onDisablePlayerMovement;
    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null) 
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;
    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null) 
        {
            onEnablePlayerMovement();
        }
    }

    public event Action<int> onExperienceGained;
    public void ExperienceGained(int experience) 
    {
        if (onExperienceGained != null) 
        {
            onExperienceGained(experience);
        }
    }

    public event Action<int> onCommunityLevelChange;
    public void CommunityLevelChange(int level) 
    {
        if (onCommunityLevelChange != null) 
        {
            onCommunityLevelChange(level);
        }
    }

    public event Action<int> onCommunityExperienceChange;
    public void CommunityExperienceChange(int experience) 
    {
        if (onCommunityExperienceChange != null) 
        {
            onCommunityExperienceChange(experience);
        }
    }
}
