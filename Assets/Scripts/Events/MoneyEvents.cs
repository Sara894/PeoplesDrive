using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoneyEvents
{
    public event Action<int> onMoneyGained;
    public void MoneyGained(int gold) 
    {
        if (onMoneyGained != null) 
        {
            onMoneyGained(gold);
        }
    }

    public event Action<int> onMoneyChange;
    public void MoneyChange(int gold) 
    {
        if (onMoneyChange != null) 
        {
            onMoneyChange(gold);
        }
    }
}
