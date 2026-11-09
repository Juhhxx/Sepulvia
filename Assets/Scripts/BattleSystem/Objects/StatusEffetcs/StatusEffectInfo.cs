using TNRD;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Battle System/New Status Effect")]
public class StatusEffectInfo : ScriptableObject, IDescriptable
{
    [field: Header("Base Status Effect Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    [field: Space(10)]
    [field: Header("Status Effect Duration Parameters")]
    [field: Space(5)]
    [field: SerializeField] public int TurnDuration { get; private set; }

    [field: Space(10)]
    [field: Header("Status Effect VFX Parameters")]
    [field: Space(5)]
    [field: SerializeField] public BattlerVFX OnEnterVFX { get; private set; }
    [field: SerializeField] public BattlerVFX OnTriggeredVFX { get; private set; }
    [field: SerializeField] public BattlerVFX OnExitVFX { get; private set; }

    [field: Space(10)]
    [field: Header("Status Effect Logic Parameters")]
    [field: Space(5)]
    [SerializeField] private SerializableInterface<IStatusEffect> _statusEffect;
    public SerializableInterface<IStatusEffect> StatusEffectLogic => _statusEffect;

    public StatusEffect Instantiate()
    {
        return new StatusEffect(this);
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }

    [SerializeField] private SerializableInterface<IDescriptable>[] _additionalDescriptions;
    public IDescriptable[] GetAdditionalDescriptions()
    {
        return _additionalDescriptions.Select(i => i.Value).ToArray();
    }
}

[Serializable]
public class StatusEffect
{
    public StatusEffect (StatusEffectInfo info)
    {
        Name = info.Name;
        Icon = info.Icon;
        Description = info.Description;
        AdditionalDescriptions = info.GetAdditionalDescriptions();

        TurnDuration = info.TurnDuration;

        OnEnterVFX = info.OnEnterVFX;
        OnTriggeredVFX = info.OnTriggeredVFX;
        OnExitVFX = info.OnExitVFX;

        _statusEffect = new SerializableInterface<IStatusEffect>(info.StatusEffectLogic.Value.Instantiate());
    }

    [field: Header("Base Status Effect Parameters")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea, ReadOnly] public string Description { get; private set; }
    public IDescriptable[] AdditionalDescriptions { get; private set; }

    [field: SerializeField, ReadOnly] public int TurnDuration { get; private set; }
    [SerializeField, ReadOnly] private int _turnsPassed = 0;
    public int TurnsPassed => _turnsPassed;
    public void TurnPassed()
    {
        _turnsPassed++;
        OnTurnPassed?.Invoke(TurnDuration - _turnsPassed);
    }
    public bool CheckIfDone()
    {
        bool result = _turnsPassed == TurnDuration;

        return result;
    }
    public void ResetTurnsPassed()
    {
        _turnsPassed = 0;
        OnTurnPassed?.Invoke(TurnDuration - _turnsPassed);
    }
    public void ChangeTurnDuration(int turns)
    {
        TurnDuration = turns;
        OnTurnPassed?.Invoke(TurnDuration - _turnsPassed);
    }
    public void Completed()
    {
        OnCompleted?.Invoke();
    }
    public event Action<int> OnTurnPassed;
    public event Action OnCompleted;

    public BattlerVFX OnEnterVFX { get; private set; }
    public BattlerVFX OnTriggeredVFX { get; private set; }
    public BattlerVFX OnExitVFX { get; private set; }

    [SerializeField, ReadOnly] private SerializableInterface<IStatusEffect> _statusEffect;
    public IStatusEffect StatusEffectLogic => _statusEffect.Value;
}

public interface IStatusEffect
{
    public event Action OnEffectTriggered;
    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect);
    public void OnUpdateEffect();
    public void OnTriggerEffect(params BattlerController[] effectTargets);
    public void OnExitEffect();
    public IStatusEffect Instantiate();
}