using System;
using UnityEngine;

public class StatusEffectStunned : IStatusEffect
{
    private BattlerController _target;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;

        float immunity = 1 - _target.StatusEffectManager.CheckImmunity(statusEffect.Name);

        statusEffect.ChangeTurnDuration(Mathf.CeilToInt(statusEffect.TurnDuration * immunity));

        _target.Character.RecoveryTime += statusEffect.TurnDuration;

        _target.SetStunned(true);
    }

    public void OnExitEffect()
    {
        _target.SetStunned(false);
    }

    public void OnTriggerEffect(params BattlerController[] effectTargets) {}

    public void OnUpdateEffect() {}
}