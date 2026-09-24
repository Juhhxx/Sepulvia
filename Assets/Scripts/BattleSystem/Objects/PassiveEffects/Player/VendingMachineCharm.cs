using System;
using UnityEngine;

public class VendingMachineCharm : IPassiveEffect
{
    [SerializeField] private float _chanceOfNotConsuming = 0.2f;
    public float ChanceOfNotConsuming => _chanceOfNotConsuming;
    
    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}