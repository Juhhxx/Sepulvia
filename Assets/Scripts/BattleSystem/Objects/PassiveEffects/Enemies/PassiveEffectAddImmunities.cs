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

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}

    public void ResetPassiveEffect() {}
}

[Serializable]
public struct ImmunityProfile
{
    public StatusEffectInfo StatusEffect;
    public float Amount;
}
