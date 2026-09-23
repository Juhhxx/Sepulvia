using System;
using UnityEngine;

public class ConsumableAddPassiveEffect : IConsumable
{
    [SerializeField] private PassiveEffectInfo _passiveEffect;
    public void OnConsumed(BattlerController user, BattlerController[] targets)
    {
        user.Character.AddPassiveEffect(_passiveEffect.Instantiate());
    }

    public void OnConsumed(Character user) {}

}
