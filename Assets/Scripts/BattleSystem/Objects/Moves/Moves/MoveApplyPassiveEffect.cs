using UnityEngine;

public class MoveApplyPassiveEffect : IMove
{
    [SerializeField] private PassiveEffectInfo _PassiveEffect;
    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        foreach(BattlerController target in targets)
        {
            target.Character.AddPassiveEffect(_PassiveEffect.Instantiate());
        }
    }
}
