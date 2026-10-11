using System;
using UnityEngine;

public class RapierPendant : IPassiveEffect
{
    [SerializeField] private StatusEffectInfo _strengthBonus;

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        target.OnDoBlock += () => target.StatusEffectManager.AddStatusEffect(_strengthBonus.Instantiate());
    }

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}

    public void ResetPassiveEffect() {}
}
