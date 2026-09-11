using System;
using UnityEngine;

public interface IStackableEffect
{
    public int MaxStack { get; set; }
    public int CurrentStack { get; }

    public void AddStack();
    public void OnAddMaxStackReached();
}