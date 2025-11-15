using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class StatModifier
{
    [field: SerializeField] public Stats StatAffected { get; private set; }

    [field: SerializeField] public int AmountAffected { get; private set; }

    [field: SerializeField] public int TurnDuration { get; private set; }

    [AllowNesting]
    [SerializeField, ReadOnly] private int _turnsPassed = 0;

    public void TurnPassed() => _turnsPassed++;
    public bool CheckIfDone() => _turnsPassed == TurnDuration + 1; // Don't count the first turn

    public StatModifier Instantiate()
    {
        return this.MemberwiseClone() as StatModifier;
    }

}