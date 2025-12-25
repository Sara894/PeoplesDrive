using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class AllQuestsCompletionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI successText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;
    
    [Header("Scene Transition")]
    [Tooltip("Scene to load after all quests are completed. Leave empty to disable auto-transition.")]
    [SerializeField] private string outroSceneName = "OutroScene";

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
        
        if (!string.IsNullOrEmpty(outroSceneName))
        {
            Debug.Log($"AllQuestsCompletionUI: Loading outro scene '{outroSceneName}'");
            SceneManager.LoadScene(outroSceneName, LoadSceneMode.Single);
        }
    }
}
