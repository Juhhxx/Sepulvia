using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectStunned : IStatusEffect, IStackableEffect
{
    private BattlerController _target;

    public int MaxStack { get; private set; }= 10;

    public int CurrentStack { get; private set; } = 0;
    [field: SerializeField] public bool ShowStack { get; private set; } = false;

    public event Action OnEffectTriggered;
    public event Action<int> OnStackChange;

    public void AddStack()
    {
        if (CurrentStack < MaxStack)
        {
            CurrentStack++;
            OnStackChange?.Invoke(CurrentStack);
        }

        if (CurrentStack == MaxStack)
        {
            OnMaxStackReached();
        }
    }
    public void RemoveStack()
    {
        if (CurrentStack > 0)
        {
            CurrentStack--;
        }
    }
    public void OnMaxStackReached() {}

    public bool CheckForStacking(BattlerController target, StatusEffect statusEffect)
    {
        var tmp = new List<StatusEffect>(target.StatusEffectManager.ActiveStatusEffects);

        tmp.Remove(statusEffect);

        foreach (StatusEffect se in tmp)
        {
            if (se.StatusEffectLogic is StatusEffectStunned stunnedEffect)
            {
                stunnedEffect.AddStack();
                se.ChangeTurnDuration(se.TurnDuration - se.TurnsPassed + statusEffect.TurnDuration);
                return true;
            }
        }

        return false;
    }

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;

        float immunity = 1 - _target.StatusEffectManager.CheckImmunity(statusEffect.Name);

        statusEffect.ChangeTurnDuration(Mathf.CeilToInt(statusEffect.TurnDuration * immunity));

        _target.Character.RecoveryTime += statusEffect.TurnDuration;

        _target.SetStunned(true);

        if (CheckForStacking(target, statusEffect))
        {
            target.StatusEffectManager.RemoveStatusEffect(statusEffect);
        }
        else
        {
            CurrentStack = 1;
            _target = target;
        }
    }

    public void OnExitEffect()
    {
        _target.SetStunned(false);
    }

    public void OnTriggerEffect(params BattlerController[] effectTargets) {}

    public void OnUpdateEffect() {}
}