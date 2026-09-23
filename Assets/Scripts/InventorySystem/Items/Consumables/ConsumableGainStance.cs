using System;
using UnityEngine;

public class ConsumableGainStance : IConsumable
{
    [SerializeField] private int _stanceAmount;
    public void OnConsumed(BattlerController user, BattlerController[] targets)
    {
        user.Character.CurrentStance += _stanceAmount;
    }

    public void OnConsumed(Character user) {}
    
}
