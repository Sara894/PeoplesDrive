using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AllQuestsCompletionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI successText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;

    [Header("Events - Hook up cutscenes, scene transitions, etc")]
    [SerializeField] private UnityEvent onAllQuestsCompleted;

    private void Start()
    {
        if (successText != null)
        {
            successText.gameObject.SetActive(false);
        }

        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onAllQuestsCompleted += OnAllQuestsCompleted;
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onAllQuestsCompleted -= OnAllQuestsCompleted;
        }
    }

    private void OnAllQuestsCompleted()
    {
        Debug.Log("AllQuestsCompletionUI: All quests completed!");
        
        if (successText != null)
        {
            StartCoroutine(ShowSuccessMessage());
        }

        onAllQuestsCompleted?.Invoke();
    }

    private IEnumerator ShowSuccessMessage()
    {
        successText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        successText.gameObject.SetActive(false);
    }
}
