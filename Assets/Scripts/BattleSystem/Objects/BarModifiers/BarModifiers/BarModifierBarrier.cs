using System;
using UnityEngine;

public class BarModifierBarrier : IBarModifier
{
    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager)
    {
        pullManager.StopMovement();
    }
}