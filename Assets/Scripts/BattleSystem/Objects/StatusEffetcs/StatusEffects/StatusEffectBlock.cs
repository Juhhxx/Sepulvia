using UnityEngine;
using System;
using System.Linq;

public class StatusEffectBlock : IStatusEffect
{
    [SerializeField] private StatusEffectInfo _stun;
    [SerializeField] private float _stanceRewardAmount = 1f;

    private BattlerController _target;
    private StatusEffect _effect;
    public event Action OnEffectTriggered;

    public void OnEnterEffect(BattlerController target, StatusEffect statusEffect)
    {
        _target = target;
        _effect = statusEffect;
    }

    public void OnExitEffect() {}

    // taget is who is blocking and effectTargets are who is being blocked
    public void OnTriggerEffect(params BattlerController[] effectTargets)
    {
        Debug.Log("BLOCK TRIGGERED");
        
        OnEffectTriggered?.Invoke();
        
        _target.StatusEffectManager.RemoveStatusEffect(_effect);

        _target.OnBlock();

        _target.Character.CurrentStance += _stanceRewardAmount;
        
        foreach (BattlerController bc in effectTargets)
        {
            bc.StatusEffectManager.AddStatusEffect(_stun.Instantiate());

            // if affected battler is immune to stun give them the recovery time of the blocked move
            if (bc.StatusEffectManager.CheckImmunity("Stun") >= 1)
            {
                Move lastmove = bc.ActionTimeline.Last().Action.Move;

                bc.Character.RecoveryTime += lastmove.RecoveryCost;
            }

            bc.ClearActions();
        }
    }

    public void OnUpdateEffect() {}

    public IStatusEffect Instantiate()
    {
        return new StatusEffectBlock(_stun, _stanceRewardAmount);
    }

    public StatusEffectBlock(StatusEffectInfo stun, float stanceReward)
    {
        _stun = stun;
        _stanceRewardAmount = stanceReward;

    }
}