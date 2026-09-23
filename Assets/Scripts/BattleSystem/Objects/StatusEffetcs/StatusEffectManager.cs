using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatusEffects => _activeStatusEffects;

    private Dictionary<string,float> _immunities = new Dictionary<string,float>();

    public void AddImmunity(string statusEffect, float percentage)
    {
        if (_immunities.ContainsKey(statusEffect))
        {
            _immunities[statusEffect] = percentage;
        }
        else _immunities.Add(statusEffect, percentage);
    }
    public void RemoveImmunity(string statusEffect)
    {
        _immunities.Remove(statusEffect);
    }
    public float CheckImmunity(string statusEffect)
    {
        return _immunities.GetValueOrDefault(statusEffect, 0);
    }

    public void AddStatusEffect(StatusEffect se, BattlerController character)
    {
        if (_immunities.ContainsKey(se.Name) && _immunities[se.Name] == 1) return;

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
