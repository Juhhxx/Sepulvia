using System;
using System.Linq;
using UnityEngine;

public class StatusEffectInterrupt : IStatusEffect
{
    [SerializeField] private int _defaultStunAmount;
    [SerializeField] private int _stanceRewardAmount;

    private BattlerController _target;
    private StatusEffect _effect;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;
        _effect = statusEffect;
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {
        _target.StatusEffectManager.RemoveStatusEffect(_effect);
        _target.Character.CurrentStance += _stanceRewardAmount;

        foreach (BattlerController bc in effectTargets)
        {
            var targetLastMove = GetLatestMove(bc);

            bc.Character.RecoveryTime += targetLastMove != null ?
                                    targetLastMove.RecoveryCost : _defaultStunAmount;
            bc.ClearActions();
        }
    }

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