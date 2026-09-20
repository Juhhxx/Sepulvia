using System;
using UnityEngine;

public class BarModifierStun : IBarModifier
{
    [SerializeField] private int _stunAmount;

    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager)
    {
        user.Character.RecoveryTime += _stunAmount;
    }
}