using System;
using UnityEngine;

public class PassiveEffectAddImmunities : IPassiveEffect
{
    [SerializeField] private ImmunityProfile[] _immunitiesToAdd;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        foreach (ImmunityProfile ip in _immunitiesToAdd)
        {
            target.StatusEffectManager.AddImmunity(ip.StatusEffect.Name, ip.Amount);
        }

        _done = true;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}

[Serializable]
public struct ImmunityProfile
{
    public StatusEffectInfo StatusEffect;
    public float Amount;
}
