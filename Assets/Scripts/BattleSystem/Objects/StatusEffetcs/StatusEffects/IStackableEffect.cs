using System;
using UnityEngine;

public interface IStackableEffect
{
    public int MaxStack { get; }
    public int CurrentStack { get; }

    public bool CheckForStacking(Character target, StatusEffect statusEffect);
    public void AddStack();
    public void RemoveStack();
    public void OnAddMaxStackReached();
}