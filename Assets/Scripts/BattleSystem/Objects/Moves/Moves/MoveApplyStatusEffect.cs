using UnityEngine;

public class MoveApplyStatusEffect : IMove
{
    [SerializeField] private StatusEffectInfo _statusEffect;
    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        resolver.DoStatusEffect(_statusEffect.Instantiate(), targets);
    }
}