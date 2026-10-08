using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusEffectManager : MonoBehaviour
{
    private BattlerController _battlerController;

    private void Awake()
    {
        _battlerController = GetComponent<BattlerController>();
    }

    // Immunities Add, Remove and Remove
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

    // Status Effects Add, Remove, Update, Cehck and Get
    [SerializeField] private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatusEffects => _activeStatusEffects;

    public event Action<StatusEffect> OnAddStatusEffect;

    public void AddStatusEffect(StatusEffect se)
    {
        if (_immunities.ContainsKey(se.Name) && _immunities[se.Name] == 1) return;

        Debug.Log($"ADDING STATUS EFFECT {se.Name} TO {_battlerController.name}", this);

        _activeStatusEffects.Add(se);
        
        OnAddStatusEffect?.Invoke(se);

        se.StatusEffectLogic.OnEnterEffect(_battlerController, se);

        _battlerController.AnimationController.DoBattlerVFX(se.OnEnterVFX);

        se.StatusEffectLogic.OnEffectTriggered += () => 
        _battlerController.AnimationController.DoBattlerVFX(se.OnTriggeredVFX);
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
        se.Completed();

        Debug.Log($"REMOVING STATUS EFFECT {se.Name}", this);

        _activeStatusEffects.Remove(se);

        se.StatusEffectLogic.OnExitEffect();

        _battlerController.AnimationController.DoBattlerVFX(se.OnEnterVFX);
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

    public void ClearStatusEffects()
    {
        foreach (StatusEffect se in new List<StatusEffect>(_activeStatusEffects))
        {
            RemoveStatusEffect(se);
        }
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
