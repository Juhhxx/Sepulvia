using System;
using UnityEngine;

public class BarModifierStun : IBarModifier
{
    [SerializeField] private int _stunAmount;

    public void OnBarModifierTriggered(int position, Character user, PullingManager pullManager)
    {
        user.RecoveryTime += _stunAmount;
    }
}