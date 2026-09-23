using UnityEngine;

public class StatusEffectBlock : IStatusEffect
{
    [SerializeField] private StatusEffectInfo _stun;
    [SerializeField] private int _stanceRewardAmount;

    private BattlerController _target;
    private StatusEffect _effect;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;
        _effect = statusEffect;
    }

    public void OnExitEffect() {}

    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {
        _target.StatusEffectManager.RemoveStatusEffect(_effect);

        _target.OnBlock();

        _target.Character.CurrentStance += _stanceRewardAmount;
        
        foreach (BattlerController bc in effectTargets)
        {
            bc.StatusEffectManager.AddStatusEffect(_stun.Instantiate(), bc);
            bc.ClearActions();
        }
    }

    public void OnUpdateEffect() {}
}