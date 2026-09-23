using System;
using UnityEngine;

public class ConsumableResetRecoveryTime : IConsumable
{
    public void OnConsumed(BattlerController user, BattlerController[] targets)
    {
        user.Character.RecoveryTime = 0;
    }

    public void OnConsumed(Character user) {}
    
}
