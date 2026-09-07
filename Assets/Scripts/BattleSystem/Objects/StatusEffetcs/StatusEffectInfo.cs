using TNRD;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Battle System/New Status Effect")]
public class StatusEffectInfo : ScriptableObject
{
    [field: Header("Base Status Effect Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    [field: SerializeField] public int TurnDuration { get; private set; }

    [SerializeField] private SerializableInterface<IStatusEffect> _statusEffect;
    public IStatusEffect StatusEffectLogic => _statusEffect.Value;

}

public class StatusEffect
{
    [field: Header("Base Status Effect Parameters")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea, ReadOnly] public string Description { get; private set; }

    [field: SerializeField, ReadOnly] public int TurnDuration { get; private set; }
    [SerializeField, ReadOnly] private int _turnsPassed = 0;
    public void TurnPassed() => _turnsPassed++;
    public bool CheckIfDone() => _turnsPassed == TurnDuration + 1; // Don't count the first turn

    [SerializeField, ReadOnly] private SerializableInterface<IStatusEffect> _statusEffect;
    public IStatusEffect StatusEffectLogic => _statusEffect.Value;
}

public interface IStatusEffect
{
    public void OnEnterEffect(Character target);
    public void OnUpdateEffect();
    public void OnTriggerEffect();
    public void OnExitEffect();
}