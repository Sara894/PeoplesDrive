using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Display Settings")]
    [SerializeField] private string prefix = "$";
    [SerializeField] private bool showPrefixInText = true;

    private void Start()
    {
        Debug.Log("MoneyDisplay: Start - Subscribing to onMoneyChange");
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.moneyEvents.onMoneyChange += UpdateMoneyDisplay;
            Debug.Log("MoneyDisplay: Successfully subscribed to onMoneyChange");
        }
        else
        {
            Debug.LogError("MoneyDisplay: GameEventsManager.instance is NULL in Start!");
        }

        if (moneyText == null)
        {
            moneyText = GetComponent<TextMeshProUGUI>();
        }

        if (moneyText == null)
        {
            Debug.LogError("MoneyDisplay: No TextMeshProUGUI component found!");
        }

        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager != null)
        {
            UpdateMoneyDisplay(moneyManager.currentMoney);
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.moneyEvents.onMoneyChange -= UpdateMoneyDisplay;
        }
    }

    private void UpdateMoneyDisplay(int money)
    {
        Debug.Log($"MoneyDisplay: Updating money display to {money}");
        
        if (moneyText != null)
        {
            if (showPrefixInText)
            {
                moneyText.text = $"{prefix}{money}";
            }
            else
            {
                moneyText.text = money.ToString();
            }
        }
        else
        {
            Debug.LogError("MoneyDisplay: moneyText is null!");
        }
    }
}
