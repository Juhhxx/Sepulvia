using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusEffectManager
{
    [SerializeField] private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatusEffects => _activeStatusEffects;
    
    private Character _character;

    public void AddStatusEffect(StatusEffect se)
    {
        _activeStatusEffects.Add(se);

        se.StatusEffectLogic.OnEnterEffect(_character, se);
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
        _activeStatusEffects.Remove(se);

        se.StatusEffectLogic.OnExitEffect();
    }

    public void UpdateStatusEffects()
    {
        foreach (StatusEffect se in _activeStatusEffects)
        {
            UpdateStatusEffect(se);
        }
    }

    private void UpdateStatusEffect(StatusEffect se)
    {
        se.TurnPassed();

        if (se.CheckIfDone())
        {
            RemoveStatusEffect(se);
            return;
        }

        se.StatusEffectLogic.OnUpdateEffect();
    }

    public bool HasStatusEffect<T>() where T : IStatusEffect
    {
        foreach (StatusEffect se in _activeStatusEffects)
        {
            if (se.StatusEffectLogic is T)
            {
                return true;
            }
        }

        return false;
    }

    public StatusEffect GetStatusEffect<T>() where T : IStatusEffect
    {
        foreach (StatusEffect se in _activeStatusEffects)
        {
            if (se.StatusEffectLogic is T)
            {
                return se;
            }
        }

        return null;
    }

    public StatusEffectManager (Character character)
    {
        _character = character;
    }

}
