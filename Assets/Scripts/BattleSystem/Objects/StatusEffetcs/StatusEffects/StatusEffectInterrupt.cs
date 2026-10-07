using System;
using System.Linq;
using UnityEngine;

public class StatusEffectInterrupt : IStatusEffect
{
    [SerializeField] private int _defaultStunAmount;

    private BattlerController _target;
    private StatusEffect _effect;
    public event Action OnEffectTriggered;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;
        _effect = statusEffect;
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {
        OnEffectTriggered?.Invoke();
        
        _target.StatusEffectManager.RemoveStatusEffect(_effect);

        var targetLastMove = GetLatestMove(_target);

        _target.Character.RecoveryTime += targetLastMove != null ?
                                targetLastMove.RecoveryCost : _defaultStunAmount;
        _target.ClearActions();
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