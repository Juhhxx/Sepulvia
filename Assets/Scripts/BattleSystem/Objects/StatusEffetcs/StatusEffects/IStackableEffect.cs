using System;
using UnityEngine;

public interface IStackableEffect
{
    public int MaxStack { get; }
    public int CurrentStack { get; }
    public bool ShowStack { get; }

    public event Action<int> OnStackChange;

    public bool CheckForStacking(BattlerController target, StatusEffect statusEffect);
    public void AddStack();
    public void RemoveStack();
    public void OnMaxStackReached();
}