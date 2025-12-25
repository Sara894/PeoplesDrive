using System.Collections;
using UnityEngine;
using TMPro;

public class UIReminderSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tutorialReminderText;
    [SerializeField] private TextMeshProUGUI questLogReminderText;

    [Header("Blink Settings")]
    [SerializeField] private float blinkOnDuration = 0.5f;
    [SerializeField] private float blinkOffDuration = 0.5f;
    [SerializeField] private int numberOfBlinks = 3;

    private bool isQuestLogReminderActive = false;

    private void Start()
    {
        InitializeReminders();
        StartReminders();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance == null)
            return;

        GameEventsManager.instance.questEvents.onStartQuest += OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onAdvanceQuest += OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onFinishQuest += OnQuestFinished;
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += OnDialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += OnDialogueFinished;
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance == null)
            return;

        GameEventsManager.instance.questEvents.onStartQuest -= OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onFinishQuest -= OnQuestFinished;
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= OnDialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
    }

    private void InitializeReminders()
    {
        if (tutorialReminderText != null)
        {
            tutorialReminderText.gameObject.SetActive(false);
        }

        if (questLogReminderText != null)
        {
            questLogReminderText.gameObject.SetActive(false);
        }
    }

    private void StartReminders()
    {
        if (tutorialReminderText != null)
        {
            StartCoroutine(BlinkTutorialReminder());
        }

        if (questLogReminderText != null)
        {
            StartCoroutine(BlinkQuestLogReminderOnce());
        }
    }

    private IEnumerator BlinkTutorialReminder()
    {
        yield return BlinkText(tutorialReminderText, numberOfBlinks);
    }

    private IEnumerator BlinkQuestLogReminderOnce()
    {
        yield return BlinkText(questLogReminderText, numberOfBlinks);
    }

    private IEnumerator BlinkText(TextMeshProUGUI textComponent, int blinkCount)
    {
        if (textComponent == null)
            yield break;

        isQuestLogReminderActive = true;

        for (int i = 0; i < blinkCount; i++)
        {
            textComponent.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkOnDuration);

            textComponent.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkOffDuration);
        }

        textComponent.gameObject.SetActive(true);

        isQuestLogReminderActive = false;
    }

    private void OnQuestStateChanged(string questId)
    {
    }

    private void OnDialogueStarted()
    {
    }

    private void OnDialogueFinished()
    {
    }

    private void OnQuestFinished(string questId)
    {
        Debug.Log($"<color=yellow>UIReminderSystem: Quest finished - {questId}. Blinking Quest Log reminder!</color>");
        
        if (questLogReminderText != null && !isQuestLogReminderActive)
        {
            StartCoroutine(BlinkText(questLogReminderText, numberOfBlinks));
        }
    }
}
