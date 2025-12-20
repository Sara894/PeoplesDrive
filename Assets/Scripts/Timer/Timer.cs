using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI failedText;
    [SerializeField] private GameObject timerContainer;

    [Header("Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float zeroDisplayDuration = 1f;
    [SerializeField] private float failedMessageDuration = 2f;

    private float remainingTime;
    private bool isTimerActive = false;
    private Quest currentTimedQuest;
    private bool hasBeenStopped = false;

    private void Start()
    {
        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }

        if (failedText != null)
        {
            failedText.gameObject.SetActive(false);
        }

        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onStartQuest += OnQuestStarted;
            GameEventsManager.instance.questEvents.onQuestStateChange += OnQuestStateChange;
            GameEventsManager.instance.questEvents.onFinishQuest += OnQuestFinished;
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onStartQuest -= OnQuestStarted;
            GameEventsManager.instance.questEvents.onQuestStateChange -= OnQuestStateChange;
            GameEventsManager.instance.questEvents.onFinishQuest -= OnQuestFinished;
        }
    }

    private void Update()
    {
        if (!isTimerActive || currentTimedQuest == null)
            return;

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (remainingTime <= 0 && isTimerActive)
        {
            remainingTime = 0;
            OnTimerExpired();
        }
    }

    private void OnQuestStarted(string questId)
    {
        Quest quest = QuestManager.instance.GetQuestById(questId);
        if (quest != null && quest.info.timeLimitInSeconds > 0)
        {
            if (currentTimedQuest != null)
            {
                Debug.LogWarning($"Timer: Attempted to start timer for '{quest.info.displayName}' but timer already running for '{currentTimedQuest.info.displayName}'. Ignoring.");
                return;
            }
            hasBeenStopped = false;
            Debug.Log($"Timer: Quest '{quest.info.displayName}' has time limit. Waiting for manual start trigger.");
        }
    }

    private void OnQuestStateChange(Quest quest)
    {
        if (hasBeenStopped)
        {
            return;
        }
        
        Debug.Log($"Timer: OnQuestStateChange called for quest {quest.info.id}, state: {quest.state}, hasTimeLimit: {quest.info.timeLimitInSeconds > 0}, currentTimedQuest: {(currentTimedQuest != null ? currentTimedQuest.info.id : "null")}");
    }

    private void OnQuestFinished(string questId)
    {
        if (currentTimedQuest != null && currentTimedQuest.info.id == questId)
        {
            StopTimer();
            StopAllCoroutines();
            Debug.Log($"Timer: Quest {questId} finished - timer stopped and fail coroutines cancelled");
        }
    }

    private void StartTimer(Quest quest)
    {
        currentTimedQuest = quest;
        remainingTime = quest.info.timeLimitInSeconds;
        isTimerActive = true;

        if (timerContainer != null)
        {
            timerContainer.SetActive(true);
        }

        if (timerText != null)
        {
            timerText.color = normalColor;
        }

        Debug.Log($"Timer: Started for quest '{quest.info.displayName}' with {remainingTime} seconds");
    }

    public void StartTimerForQuest(string questId)
    {
        if (currentTimedQuest != null)
        {
            Debug.LogWarning($"Timer: Cannot start - timer already running for '{currentTimedQuest.info.id}'");
            return;
        }

        Quest quest = QuestManager.instance.GetQuestById(questId);
        if (quest != null && quest.info.timeLimitInSeconds > 0)
        {
            StartTimer(quest);
            Debug.Log($"Timer: Manually started for quest '{questId}'");
        }
        else
        {
            Debug.LogWarning($"Timer: Quest '{questId}' not found or has no time limit");
        }
    }

    private void StopTimer()
    {
        isTimerActive = false;
        hasBeenStopped = true;
        currentTimedQuest = null;

        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }

        Debug.Log("Timer: Stopped");
    }

    public void StopTimerImmediately()
    {
        isTimerActive = false;
        remainingTime = 0;
        hasBeenStopped = true;
        StopAllCoroutines();
        
        if (timerText != null)
        {
            timerText.color = normalColor;
        }
        
        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }
        
        if (failedText != null)
        {
            failedText.gameObject.SetActive(false);
        }
        
        currentTimedQuest = null;
        
        Debug.Log("Timer: Stopped immediately (called externally) - UI hidden and reset");
    }

    public void AddBonusTime(float bonusSeconds)
    {
        if (!isTimerActive || currentTimedQuest == null)
        {
            Debug.LogWarning($"Timer: Cannot add bonus time - timer is not active!");
            return;
        }

        remainingTime += bonusSeconds;
        Debug.Log($"Timer: Added {bonusSeconds} bonus seconds! New remaining time: {remainingTime:F1}s");
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimerExpired()
    {
        isTimerActive = false;

        if (timerText != null)
        {
            timerText.color = warningColor;
            timerText.text = "00:00";
        }

        Debug.Log($"Timer: Expired for quest '{currentTimedQuest.info.displayName}'");

        if (currentTimedQuest != null)
        {
            currentTimedQuest.MarkAsFailing();
            DisableQuestSteps(currentTimedQuest.info.id);
        }

        StartCoroutine(HandleQuestFailure());
    }

    private void DisableQuestSteps(string questId)
    {
        if (QuestManager.instance == null)
            return;

        QuestStep[] questSteps = FindObjectsOfType<QuestStep>();
        foreach (QuestStep step in questSteps)
        {
            if (step.questId == questId)
            {
                step.gameObject.SetActive(false);
                Debug.Log($"Timer: Disabled quest step {step.name} for failed quest {questId}");
            }
        }
    }

    private IEnumerator HandleQuestFailure()
    {
        string failingQuestId = currentTimedQuest != null ? currentTimedQuest.info.id : null;

        yield return new WaitForSeconds(zeroDisplayDuration);

        if (currentTimedQuest == null || currentTimedQuest.info.id != failingQuestId)
        {
            Debug.Log("Timer: Quest was completed or changed during fail delay - aborting fail sequence");
            yield break;
        }

        Quest quest = QuestManager.instance.GetQuestById(failingQuestId);
        if (quest.state == QuestState.FINISHED || quest.state == QuestState.CAN_FINISH)
        {
            Debug.Log($"Timer: Quest {failingQuestId} already completed - aborting fail");
            yield break;
        }

        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }

        if (failedText != null)
        {
            failedText.gameObject.SetActive(true);
            failedText.color = warningColor;
            failedText.text = "FAILED";
        }

        yield return new WaitForSeconds(failedMessageDuration);

        if (currentTimedQuest == null || currentTimedQuest.info.id != failingQuestId)
        {
            Debug.Log("Timer: Quest was completed during fail message - aborting fail event");
            if (failedText != null)
            {
                failedText.gameObject.SetActive(false);
            }
            yield break;
        }

        quest = QuestManager.instance.GetQuestById(failingQuestId);
        if (quest.state == QuestState.FINISHED || quest.state == QuestState.CAN_FINISH)
        {
            Debug.Log($"Timer: Quest {failingQuestId} completed during fail message - aborting fail event");
            if (failedText != null)
            {
                failedText.gameObject.SetActive(false);
            }
            yield break;
        }

        if (failedText != null)
        {
            failedText.gameObject.SetActive(false);
        }

        if (GameEventsManager.instance != null)
        {
            currentTimedQuest = null;
            GameEventsManager.instance.questEvents.QuestFailed(failingQuestId);
            Debug.Log($"Timer: Quest '{failingQuestId}' failed due to timeout");
        }
    }
}