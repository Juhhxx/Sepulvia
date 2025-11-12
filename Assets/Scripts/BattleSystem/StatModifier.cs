using System;
using UnityEngine;

[Serializable]
public class StatModifier
{
    [field: SerializeField] public Stats StatAffected { get; private set; }

    [field: SerializeField] public int AmountAffected { get; private set; }

    [field: SerializeField] public int TurnDuration { get; private set; }
}