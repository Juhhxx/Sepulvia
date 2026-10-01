using TNRD;
using UnityEngine;
using NaughtyAttributes;
using System;

[CreateAssetMenu(fileName = "BarModifier", menuName = "Battle System/New Bar Modifier")]
public class BarModifierInfo : ScriptableObject
{
    [field: Header("Base Bar Modifier Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Sprite TimelineIndicator { get; private set; }
    [field: SerializeField] public GameObject BarEffectPrefab { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    [field: Space(10)]
    [field: Header("Bar Modifier Trigger & Duration Parameters")]
    [field: Space(5)]
    [field: SerializeField] public BarModifierTrigger Trigger { get; private set; }
    [field: SerializeField] public int TurnDuration { get; private set; }
    [field: SerializeField] public bool DestroyOnUse { get; private set; }

    [field: Space(10)]
    [field: Header("Bar Modifier Logic Parameters")]
    [field: Space(5)]
    [SerializeField] private SerializableInterface<IBarModifier> _barModifier;
    public SerializableInterface<IBarModifier> BarModifierLogic => _barModifier;

    public BarModifier Instantiate()
    {
        return new BarModifier(this);
    }
}


public class BarModifier
{
    public BarModifier(BarModifierInfo info)
    {
        Name = info.Name;
        Icon = info.Icon;
        TimelineIndicator = info.TimelineIndicator;
        BarEffectPrefab = info.BarEffectPrefab;
        Description = info.Description;

        Trigger = info.Trigger;
        TurnDuration = info.TurnDuration;
        DestroyOnUse = info.DestroyOnUse;

        _barModifier = info.BarModifierLogic;
    }

    [field: Header("Base Bar Modifier Parameters")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; }
    [field: SerializeField] public Sprite TimelineIndicator { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject BarEffectPrefab { get; private set; }
    [field: SerializeField, TextArea, ReadOnly] public string Description { get; private set; }


    [field: SerializeField, ReadOnly] public BarModifierTrigger Trigger { get; private set; }

    [field: SerializeField, ReadOnly] public int TurnDuration { get; private set; }
    [SerializeField, ReadOnly] private int _turnsPassed = 0;
    public void TurnPassed()
    {
        var oldDuration = TurnDuration - _turnsPassed;

        _turnsPassed++;

        var newDuration = TurnDuration - _turnsPassed;

        OnTurnPassed?.Invoke(newDuration, oldDuration);
    }
    public bool CheckIfDone() => _turnsPassed == TurnDuration + 1; // Don't count the first turn
    public void ResetTurnsPassed() => _turnsPassed = 0;
    public Action<int, int> OnTurnPassed;
    public void Completed() => OnCompleted?.Invoke();
    public Action OnCompleted;

    [field: SerializeField, ReadOnly] public bool DestroyOnUse { get; private set; }

    [SerializeField, ReadOnly] private SerializableInterface<IBarModifier> _barModifier;
    public IBarModifier BarModifierLogic => _barModifier.Value;
}

public interface IBarModifier
{
    public void OnBarModifierTriggered(int position, BattlerController user, PullingManager pullManager);
}
