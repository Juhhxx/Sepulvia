using System;
using UnityEngine;

public class PassiveEffectAddImmunities : IPassiveEffect
{
    [SerializeField] private ImmunityProfile[] _immunitiesToAdd;

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        foreach (ImmunityProfile ip in _immunitiesToAdd)
        {
            target.StatusEffectManager.AddImmunity(ip.StatusEffect.Name, ip.Amount);
        }
    }

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}

[Serializable]
public struct ImmunityProfile
{
    public StatusEffectInfo StatusEffect;
    public float Amount;
}
