using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

public class StatusEffectCurse : IStatusEffect, IStackableEffect
{
    [field: SerializeField] public int MaxStack { get; private set; }
    [field: SerializeField, ReadOnly] public int CurrentStack { get; private set; }

    [SerializeField] private StatusEffectInfo _stun;
    private BattlerController _target;
    private StatusEffect _effect;

    public void AddStack()
    {
        if (CurrentStack < MaxStack)
        {
            CurrentStack++;
        }

        if (CurrentStack == MaxStack)
        {
            OnAddMaxStackReached();
        }
    }

    public void RemoveStack()
    {
        if (CurrentStack > 0)
        {
            CurrentStack--;
        }
    }

    public void OnAddMaxStackReached()
    {
        _target.StatusEffectManager.AddStatusEffect(_stun.Instantiate(), _target);

        _target.StatusEffectManager.RemoveStatusEffect(_effect);
    }

    public bool CheckForStacking(BattlerController target, StatusEffect statusEffect)
    {
        var tmp = new List<StatusEffect>(target.StatusEffectManager.ActiveStatusEffects);

        tmp.Remove(statusEffect);

        foreach (StatusEffect se in tmp)
        {
            if (se.StatusEffectLogic is StatusEffectCurse curseEffect)
            {
                curseEffect.AddStack();
                se.ResetTurnsPassed();
                return true;
            }
        }

        return false;
    }

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        if (CheckForStacking(target, statusEffect))
        {
            target.StatusEffectManager.RemoveStatusEffect(statusEffect);
        }
        else
        {
            CurrentStack = 1;
            _target = target;
            _effect = statusEffect;
        }
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect(params BattlerController[] effectTargets) {}

    public void OnUpdateEffect() {}
}