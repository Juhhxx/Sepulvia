using System;
using UnityEngine;

public class BarModifierStun : IBarModifier
{
    [SerializeField] private StatusEffectInfo _stun;
    [SerializeField] private StatusEffectInfo _interrupt;

    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager)
    {
        user.StatusEffectManager.AddStatusEffect(_interrupt.Instantiate());
        user.StatusEffectManager.AddStatusEffect(_stun.Instantiate());
    }
}