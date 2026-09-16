using System;
using UnityEngine;

public class BarModifierBarrier : IBarModifier
{
    public void OnBarModifierTriggered(int position, Character user, PullingManager pullManager)
    {
        pullManager.StopMovement();
    }
}