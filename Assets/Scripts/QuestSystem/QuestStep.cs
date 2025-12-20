using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    public string questId { get; private set; }
    private int stepIndex;

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            Quest quest = QuestManager.instance.GetQuestById(questId);
            if (quest != null && quest.isFailing)
            {
                Debug.Log($"Quest step prevented from finishing - quest {questId} is failing");
                return;
            }

            isFinished = true;
            GameEventsManager.instance.questEvents.AdvanceQuest(questId);
            
            if (this.gameObject.name.Contains("(Clone)"))
            {
                Destroy(this.gameObject);
                Debug.Log($"QuestStep: Destroyed instantiated clone {this.gameObject.name}");
            }
            else
            {
                this.gameObject.SetActive(false);
                Debug.Log($"QuestStep: Disabled scene-placed quest step {this.gameObject.name}");
            }
        }
    }

    protected void ChangeState(string newState, string newStatus)
    {
        GameEventsManager.instance.questEvents.QuestStepStateChange(
            questId, 
            stepIndex, 
            new QuestStepState(newState, newStatus)
        );
    }

    protected void StopQuestTimer()
    {
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.StopTimerImmediately();
            Debug.Log($"QuestStep: Timer stopped immediately for quest {questId}");
        }
    }

    protected void StartQuestTimer()
    {
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.StartTimerForQuest(questId);
            Debug.Log($"QuestStep: Timer start requested for quest {questId}");
        }
    }

    protected abstract void SetQuestStepState(string state);
}
