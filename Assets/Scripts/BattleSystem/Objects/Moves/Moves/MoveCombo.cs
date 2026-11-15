using System;
using UnityEngine;

public class MoveCombo : IMove
{
    [SerializeField] private MoveInfo[] _movesToCombo;

    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        foreach (MoveInfo mi in _movesToCombo)
        {
            user.QueueAction(new BattleAction(user, targets, mi.Instantiate()));
        }
    }

    public int GetTotalRecoveryCost()
    {
        int totalCost = 0;
        foreach (MoveInfo mi in _movesToCombo)
        {
            totalCost += mi.RecoveryCost;
        }
        return totalCost;
    }
}