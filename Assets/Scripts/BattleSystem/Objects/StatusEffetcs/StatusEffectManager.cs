using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusEffectManager
{
    [SerializeField] private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    private Character _character;

    public void AddStatusEffect(StatusEffect se)
    {
        if (se.StatusEffectLogic is IStackableEffect)
        {
            var stackableEffect = _activeStatusEffects.Find((s) => s.StatusEffectLogic is IStackableEffect);

            if (stackableEffect != null)
            {
                (stackableEffect.StatusEffectLogic as IStackableEffect)?.AddStack();
                return;
            }
            else
            {
                _activeStatusEffects.Add(se);
            }
        }
        else _activeStatusEffects.Add(se);

        se.StatusEffectLogic.OnEnterEffect(_character);
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
