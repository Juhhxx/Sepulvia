using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatusEffects => _activeStatusEffects;


    public void AddStatusEffect(StatusEffect se, BattlerController character)
    {
        _activeStatusEffects.Add(se);

        se.StatusEffectLogic.OnEnterEffect(character, se);
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
        _activeStatusEffects.Remove(se);

        se.StatusEffectLogic.OnExitEffect();
    }

    public void UpdateStatusEffects()
    {
        foreach (StatusEffect se in new List<StatusEffect>(_activeStatusEffects))
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

}
