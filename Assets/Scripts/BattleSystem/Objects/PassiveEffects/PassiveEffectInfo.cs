using TNRD;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "PassiveEffect", menuName = "Battle System/New Passive Effect")]
public class PassiveEffectInfo : ScriptableObject
{
    [field: Header("Base Passive Effect Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    [SerializeField] private SerializableInterface<IPassiveEffect> _passiveEffect;
    public IPassiveEffect PassiveEffectLogic => _passiveEffect.Value;

}

public interface IPassiveEffect
{
    public void OnEnterEffect(Character target);
    public void OnUpdateEffect();
    public void OnTriggerEffect();
    public void OnExitEffect();
}