using UnityEngine;
using System;

public class StatusEffectPullStrenghtBoost : IStatusEffect
{
    [SerializeField] private int _boostAmount;
    private StatModifier _modifier;
    private Character _target;

    public void OnEnterEffect(Character target)
    {
        _target = target;
        _modifier = new StatModifier(Stats.PullStrength, _boostAmount);
        target.AddModifier(_modifier);
    }   

    public void OnUpdateEffect() {}

    public void OnTriggerEffect() {}

    public void OnExitEffect()
    {
        _target.RemoveModifier(_modifier);
    }
}
