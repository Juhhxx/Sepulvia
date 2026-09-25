using System;
using System.Linq;
using UnityEngine;

public class MoveComboEnemy : IMove
{
    [SerializeField] private MoveInfo[] _movesToCombo;

    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        EnemyBattleAI battleAI = user.GetComponent<EnemyBattleAI>();

        var playerParty = targets.Where((c) => c.Character is Player).ToArray();
        var enemyParty = targets.Where((c) => c.Character is Enemy).ToArray();

        foreach (MoveInfo mi in _movesToCombo)
        {
            var moveTargets = battleAI.ChooseTargets(user, enemyParty, playerParty, mi.Targeting);
            user.QueueAction(new BattleAction(user, moveTargets, mi.Instantiate()));
        }
    }
}