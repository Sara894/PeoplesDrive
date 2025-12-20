using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> onStartQuest;
    public void StartQuest(string id)
    {
        if (onStartQuest != null)
        {
            onStartQuest(id);
        }
    }

    public event Action<string> onAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(id);
        }
    }

    public event Action<string> onFinishQuest;
    public void FinishQuest(string id)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(id);
        }
    }

    public event Action<Quest> onQuestStateChange;
    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }

    public event Action<string, int, QuestStepState> onQuestStepStateChange;
    public void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        if (onQuestStepStateChange != null)
        {
            onQuestStepStateChange(id, stepIndex, questStepState);
        }
    }

    public event Action<string> onQuestFailed;
    public void QuestFailed(string id)
    {
        Debug.Log($"QuestEvents.QuestFailed called with id: {id}");
        if (onQuestFailed != null)
        {
            Debug.Log($"QuestEvents.QuestFailed: Invoking {onQuestFailed.GetInvocationList().Length} subscribers");
            onQuestFailed(id);
        }
        else
        {
            Debug.LogWarning("QuestEvents.QuestFailed: No subscribers to onQuestFailed event!");
        }
    }

    public event Action<string> onQuestStartBlocked;
    public void QuestStartBlocked(string id)
    {
        if (onQuestStartBlocked != null)
        {
            onQuestStartBlocked(id);
        }
    }

    public event Action onAllQuestsCompleted;
    public void AllQuestsCompleted()
    {
        Debug.Log("QuestEvents.AllQuestsCompleted called - all quests finished!");
        if (onAllQuestsCompleted != null)
        {
            onAllQuestsCompleted();
        }
    }
}
