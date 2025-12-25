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

    [Header("Quest Log Repeat Settings")]
    [SerializeField] private float questLogRepeatMinInterval = 15f;
    [SerializeField] private float questLogRepeatMaxInterval = 20f;
    [SerializeField] private float initialQuestLogDelay = 5f;

    private bool isQuestLogReminderActive = false;
    private Coroutine questLogReminderCoroutine;

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
        GameEventsManager.instance.questEvents.onFinishQuest += OnQuestStateChanged;
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += OnDialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += OnDialogueFinished;
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance == null)
            return;

        GameEventsManager.instance.questEvents.onStartQuest -= OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= OnQuestStateChanged;
        GameEventsManager.instance.questEvents.onFinishQuest -= OnQuestStateChanged;
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
            questLogReminderCoroutine = StartCoroutine(RepeatQuestLogReminder());
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

    private IEnumerator RepeatQuestLogReminder()
    {
        yield return new WaitForSeconds(initialQuestLogDelay);

        while (true)
        {
            float randomInterval = Random.Range(questLogRepeatMinInterval, questLogRepeatMaxInterval);
            yield return new WaitForSeconds(randomInterval);

            if (ShouldShowQuestLogReminder())
            {
                yield return BlinkText(questLogReminderText, numberOfBlinks);
            }
        }
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

    private bool ShouldShowQuestLogReminder()
    {
        if (GameEventsManager.instance == null)
            return false;

        InputEventContext currentContext = GameEventsManager.instance.inputEvents.inputEventContext;

        bool isInDialogue = currentContext == InputEventContext.DIALOGUE;
        bool isInQuestLog = currentContext == InputEventContext.QUEST_LOG;
        bool isInTutorial = currentContext == InputEventContext.TUTORIAL;
        bool hasActiveQuest = HasActiveQuest();

        bool canShow = !isInDialogue && !isInQuestLog && !isInTutorial && !hasActiveQuest;

        Debug.Log($"UIReminderSystem: ShouldShowQuestLogReminder = {canShow} (dialogue: {isInDialogue}, questLog: {isInQuestLog}, tutorial: {isInTutorial}, activeQuest: {hasActiveQuest})");

        return canShow;
    }

    private bool HasActiveQuest()
    {
        if (QuestManager.instance == null)
            return false;

        return QuestManager.instance.HasActiveQuest();
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
}
