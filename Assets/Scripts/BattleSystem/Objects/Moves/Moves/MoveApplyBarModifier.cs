using UnityEngine;
using System;

public class MoveApplyBarModifier : IMove
{
    [SerializeField] private BarModifierInfo _modifier;
    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        resolver.DoBarModifier(_modifier);
    }
}
