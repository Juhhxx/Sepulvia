using System;
using UnityEngine;

public class VendingMachineCharm : IPassiveEffect
{
    [SerializeField] private float _chanceOfNotConsuming = 0.2f;
    public float ChanceOfNotConsuming => _chanceOfNotConsuming;
    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}