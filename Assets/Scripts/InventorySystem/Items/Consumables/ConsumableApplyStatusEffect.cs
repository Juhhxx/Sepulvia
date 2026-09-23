using System;
using UnityEngine;

public class ConsumableApplyStatusEffect : IConsumable
{
    [SerializeField] private StatusEffectInfo _statusEffect;
    public void OnConsumed(BattlerController user, BattlerController[] targets)
    {
        user.StatusEffectManager.AddStatusEffect(_statusEffect.Instantiate(), user);
    }

    public void OnConsumed(Character user) {}

}
