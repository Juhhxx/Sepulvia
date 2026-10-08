using System;
using UnityEngine;

public class StatusEffectAddImmunity : IStatusEffect
{
    [SerializeField] private ImmunityProfile[] _immunitiesToAdd;

    public event Action OnEffectTriggered;
    private BattlerController _target;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;

        foreach (ImmunityProfile ip in _immunitiesToAdd)
        {
            _target.StatusEffectManager.AddImmunity(ip.StatusEffect.Name, ip.Amount);
        }
    }

    public void OnUpdateEffect()
    {

    }
    public void OnExitEffect()
    {
        foreach (ImmunityProfile ip in _immunitiesToAdd)
        {
            _target.StatusEffectManager.RemoveImmunity(ip.StatusEffect.Name);
        }
    }

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {
        
    }
}