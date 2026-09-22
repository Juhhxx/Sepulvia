using System;
using UnityEngine;

public class RapierPendant : IPassiveEffect
{
    [SerializeField] private StatusEffectInfo _strengthBonus;
    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager)
    {
        target.OnDoBlock += () => target.StatusEffectManager.AddStatusEffect(_strengthBonus.Instantiate(), target);
    }

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
