using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectNone : IStatusEffect
{
    public event Action OnEffectTriggered;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {

    }

    public void OnExitEffect()
    {

    }

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {

    }

    public void OnUpdateEffect()
    {

    }

}