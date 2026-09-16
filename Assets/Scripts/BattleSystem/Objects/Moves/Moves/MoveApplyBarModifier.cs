using UnityEngine;
using System;

public class MoveApplyBarModifier : IMove
{
    [SerializeField] private BarModifierInfo _modifier;
    public void OnDoMove(Character user, Character[] targets, BattleResolver resolver)
    {
        resolver.DoBarModifier(_modifier);
    }
}
