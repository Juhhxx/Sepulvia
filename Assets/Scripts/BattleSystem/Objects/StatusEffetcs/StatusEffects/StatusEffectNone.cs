using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectNone : IStatusEffect, IStackableEffect
{
    public int MaxStack { get; private set; }= 0;

    public int CurrentStack { get; private set; } = 0;

    public event Action OnEffectTriggered;
    public event Action<int> OnStackChange;

    public void AddStack()
    {

    }

    public bool CheckForStacking(BattlerController target, StatusEffect statusEffect)
    {
        return false;
    }

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {

    }

    public void OnExitEffect()
    {

    }

    public void OnMaxStackReached()
    {

    }

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {

    }

    public void OnUpdateEffect()
    {

    }

    public void RemoveStack()
    {
        
    }
}