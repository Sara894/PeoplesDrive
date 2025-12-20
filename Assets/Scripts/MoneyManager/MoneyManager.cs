using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingMoney = 5;

    public int currentMoney { get; private set; }

    private void Awake()
    {
        currentMoney = startingMoney;
    }

    private void OnEnable() 
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.moneyEvents.onMoneyGained += MoneyGained;
        }
    }

    private void OnDisable() 
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.moneyEvents.onMoneyGained -= MoneyGained;
        }
    }

    private void Start()
    {
        GameEventsManager.instance.moneyEvents.MoneyChange(currentMoney);
    }

    private void MoneyGained(int money) 
    {
        Debug.Log($"MoneyManager.MoneyGained called with {money}. Current money before: {currentMoney}");
        currentMoney += money;
        Debug.Log($"MoneyManager: Firing MoneyChange event with new value: {currentMoney}");
        GameEventsManager.instance.moneyEvents.MoneyChange(currentMoney);
    }
}
