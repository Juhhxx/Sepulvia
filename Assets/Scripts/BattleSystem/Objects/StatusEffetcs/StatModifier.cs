using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class StatModifier
{
    [field: SerializeField] public Stats StatAffected { get; private set; }

    [field: SerializeField] public int AmountAffected { get; private set; }

    public StatModifier Instantiate()
    {
        return this.MemberwiseClone() as StatModifier;
    }

}
