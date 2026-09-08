using UnityEngine;

public class MoveApplyStatusEffect : IMove
{
    [SerializeField] private StatusEffectInfo _statusEffect;
    public void OnDoMove(Character user, Character[] targets, BattleResolver resolver)
    {
        resolver.DoStatusEffect(_statusEffect.Instantiate(), targets);
    }
}