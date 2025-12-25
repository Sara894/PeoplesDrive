using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
{
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;

    private int choiceIndex = -1;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    public void SetChoiceText(string choiceTextString)
    {
        choiceText.text = choiceTextString;
    }

    public void SetChoiceIndex(int choiceIndex)
    {
        this.choiceIndex = choiceIndex;
    }

    public void SelectButton()
    {
        button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Navigation only - update which choice is highlighted
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
    }

    private void OnButtonClicked()
    {
        Debug.Log($"<color=orange>DialogueChoiceButton: Button clicked! Choice index: {choiceIndex}</color>");
        // Update the choice index
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
        // Confirm the choice (same as pressing E/Enter)
        GameEventsManager.instance.inputEvents.SubmitPressed();
    }
}
