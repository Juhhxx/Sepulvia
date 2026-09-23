using System;
using UnityEngine;

public class BarModifierStun : IBarModifier
{
    [SerializeField] private StatusEffectInfo _stun;

    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager)
    {
        user.StatusEffectManager.AddStatusEffect(_stun.Instantiate(), user);
    }
}