using System;
using NaughtyAttributes;
using UnityEngine;

public class BarModifierCursed : IBarModifier
{
    [SerializeField] private StatusEffectInfo _curse;

    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager)
    {
        user.StatusEffectManager.AddStatusEffect(_curse.Instantiate(), user);
    }
}
