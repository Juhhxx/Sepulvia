using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class BarModifier
{
    [Header("Bar Modifier Parameters")]
    [field: Space(5)]
    [field: SerializeField] public BarModifierType Type { get; private set; }
    [field: SerializeField] public BarModifierTrigger Trigger { get; private set; }

    [field: Space(10)]
    [Header("Aesthetic Parameters")]
    [field: Space(5)]
    [field: SerializeField] public GameObject BarEffectPrefab { get; private set; }
 
    [field: Space(10)]
    [Header("Duration Parameters")]
    [field: Space(5)]
    [field: SerializeField] public int TurnDuration { get; private set; }
    [field: SerializeField] public bool DestroyOnUse { get; private set; }

    [field: Space(10)]
    [Header("Pull Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", BarModifierType.GravityPull)]
    [field: SerializeField] public int GraviyStrength { get; private set; }

    private int _turnsPassed = 0;

    public void TurnPassed() => _turnsPassed++;
    public bool CheckIfDone() => _turnsPassed == TurnDuration + 1; // Don't count the first turn
    public bool CheckIfAlmostDone() => _turnsPassed == TurnDuration ;

    public BarModifier Instantiate()
    {
        return this.MemberwiseClone() as BarModifier;
    }
}