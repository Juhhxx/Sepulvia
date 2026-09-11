using UnityEngine;

public class StatusEffectBlock : IStatusEffect
{
    [SerializeField] private int _stunAmount;
    public int StunAmount => _stunAmount;

    [SerializeField] private int _rewardAmount;
    public int RewardAmount => _rewardAmount;

    public void OnEnterEffect(Character target) {}

    public void OnExitEffect() {}

    public void OnTriggerEffect() {}

    public void OnUpdateEffect() {}
}