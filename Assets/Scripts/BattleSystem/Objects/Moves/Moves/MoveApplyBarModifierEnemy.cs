using UnityEngine;
using System;

public class MoveApplyBarModifierEnemy : IMove
{
    [SerializeField] private BarModifierInfo _modifier;
    [SerializeField] private ModifierApplyType _whereToApply;
    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        EnemyBattleAI enemyAI = user.GetComponent<EnemyBattleAI>();

        int barIndex = enemyAI.ChooseBarSection(_whereToApply);
        
        resolver.ApplyBarModifier(barIndex, _modifier);
    }
}

public enum ModifierApplyType { BetweenSoulAndLeft, BetweenSoulAndRight, LeftSide, RightSide, Anywhere }

