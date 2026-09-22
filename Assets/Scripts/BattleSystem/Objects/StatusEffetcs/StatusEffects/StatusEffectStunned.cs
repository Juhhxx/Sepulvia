using System;
using UnityEngine;

public class StatusEffectStunned : IStatusEffect
{
    private BattlerController _target;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;

        _target.SetStunned(true);
    }

    public void OnExitEffect()
    {
        _target.SetStunned(false);
    }

    public void OnTriggerEffect(params BattlerController[] effectTargets) {}

    public void OnUpdateEffect() {}
}