using UnityEngine;
using System;

public class StatusEffectPullStrenghtBoost : IStatusEffect
{
    [SerializeField] private int _boostAmount;
    private StatModifier _modifier;
    private BattlerController _target;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;
        _modifier = new StatModifier(Stats.PullStrength, _boostAmount);
        target.Character.AddModifier(_modifier);
    }   

    public void OnUpdateEffect() {}

    public void OnTriggerEffect() {}

    public void OnExitEffect()
    {
        _target.Character.RemoveModifier(_modifier);
    }
}
