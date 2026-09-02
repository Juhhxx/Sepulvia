using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();

    public void AddStatusEffect(StatusEffect se)
    {
        _activeStatusEffects.Add(se);

        se.StatusEffectLogic.OnEnterEffect(gameObject);
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

}
