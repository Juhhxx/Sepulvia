using System;
using UnityEngine;

[Serializable]
public class BarModifier
{
    [field: SerializeField] public BarModifierTypes Type { get; private set; }

    [field: SerializeField] public Color Color { get; private set; }

    [field: SerializeField] public int TurnDuration { get; private set; }
    [field: SerializeField] public bool DestroyOnUse { get; private set; }

    private int _turnsPassed = 0;

    public void TurnPassed() => _turnsPassed++;
    public bool CheckIfDone() => _turnsPassed == TurnDuration + 1; // Don't count the first turn
}