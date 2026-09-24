using System;
using UnityEngine;

public class RapierPendant : IPassiveEffect
{
    [SerializeField] private StatusEffectInfo _strengthBonus;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        target.OnDoBlock += () => target.StatusEffectManager.AddStatusEffect(_strengthBonus.Instantiate(), target);

        _done = true;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
