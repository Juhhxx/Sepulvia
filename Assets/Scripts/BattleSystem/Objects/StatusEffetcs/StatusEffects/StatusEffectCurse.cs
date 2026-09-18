using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

public class StatusEffectCurse : IStatusEffect, IStackableEffect
{
    [field: SerializeField] public int MaxStack { get; private set; }
    [field: SerializeField, ReadOnly] public int CurrentStack { get; private set; }

    [SerializeField] private int _stunAmount = 2;
    private Character _target;
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
        _target.RecoveryTime += _stunAmount;

        _target.StatusEffectManager.RemoveStatusEffect(_effect);
    }

    public bool CheckForStacking(Character target, StatusEffect statusEffect)
    {
        var tmp = new List<StatusEffect>(target.StatusEffectManager.ActiveStatusEffects);

        tmp.Remove(statusEffect);

        foreach (StatusEffect se in tmp)
        {
            if (se.StatusEffectLogic is StatusEffectCurse curseEffect)
            {
                curseEffect.AddStack();
                return true;
            }
        }

        return false;
    }

    public void OnEnterEffect(Character target, StatusEffect statusEffect)
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

    public void OnTriggerEffect() {}

    public void OnUpdateEffect() {}
}