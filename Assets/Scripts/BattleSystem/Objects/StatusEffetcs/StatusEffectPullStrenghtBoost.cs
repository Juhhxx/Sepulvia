using UnityEngine;
using System;

public class StatusEffectPullStrenghtBoost : IStatusEffect
{
    [SerializeField] private int _boostAmount;

    public void OnEnterEffect(GameObject go)
    {
        // Implementation for when the status effect is applied
    }

    public void OnUpdateEffect()
    {
        // Implementation for when the status effect is updated
    }

    public void OnTriggerEffect()
    {
        // Implementation for when the status effect is triggered
    }

    public void OnExitEffect()
    {
        // Implementation for when the status effect is removed
    }
}
