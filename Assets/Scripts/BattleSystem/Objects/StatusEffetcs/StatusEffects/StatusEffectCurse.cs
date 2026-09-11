using UnityEngine;

public class StatusEffectCurse : IStatusEffect, IStackableEffect
{
    public int MaxStack { get; set; }
    public int CurrentStack { get; private set; }

    [SerializeField] private int _stunAmount = 2;
    private Character _target;

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

    public void OnAddMaxStackReached()
    {
        _target.RecoveryTime += _stunAmount;
    }

    public void OnEnterEffect(Character target)
    {
        AddStack();
        _target = target;
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect() {}

    public void OnUpdateEffect() {}
}