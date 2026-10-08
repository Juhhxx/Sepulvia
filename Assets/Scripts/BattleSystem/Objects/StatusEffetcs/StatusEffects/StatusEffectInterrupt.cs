using System;
using System.Linq;
using UnityEngine;

public class StatusEffectInterrupt : IStatusEffect
{
    [SerializeField] private int _defaultStunAmount;

    public event Action OnEffectTriggered;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    { 
        target.StatusEffectManager.RemoveStatusEffect(statusEffect);

        var targetLastMove = GetLatestMove(target);

        int recoveryReset = targetLastMove != null ?
                    targetLastMove.RecoveryCost : _defaultStunAmount;

        recoveryReset -= target.Character.RecoveryTime;

        if (recoveryReset < 0) recoveryReset = 0;

        target.Character.RecoveryTime += recoveryReset;

        target.ClearActions();
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect(params BattlerController[] effectTargets) {}

    private Move GetLatestMove(BattlerController target)
    {
        Move move = null;
        int i = target.ActionTimeline.Count;

        while (move == null)
        {
            i--;

            if (i < 0) return null;

            var tmp = target.ActionTimeline[i].Action.Move;

            if (tmp != null) move = tmp;
        }

        if (move.Type.HasFlag(MoveTypes.Combo) || move.Type.HasFlag(MoveTypes.PartOfCombo))
        {
            if (target.HasActions())
            {
                move = target.QueuedActions.Last().Move;
            }
        }

        return move;
    }

    public void OnUpdateEffect() {}
}