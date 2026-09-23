using System;
using UnityEngine;

public class ConsumableStunEnemies : IConsumable
{
    [SerializeField] private StatusEffectInfo _stun;
    public void OnConsumed(BattlerController user, BattlerController[] targets)
    {
        foreach (BattlerController target in targets)
        {
            target.StatusEffectManager.AddStatusEffect(_stun.Instantiate(), target);
        }
    }

    public void OnConsumed(Character user) {}

}
