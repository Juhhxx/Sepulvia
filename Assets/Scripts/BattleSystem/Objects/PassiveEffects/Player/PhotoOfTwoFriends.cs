using UnityEngine;

public class PhotoOfTwoFriends : IPassiveEffect
{
    [SerializeField] private float _stanceGain;
    private BattlerController _target;

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        _target = target;
    }

    public void OnBeginTurnEffect(BattlerController[] others)
    {
        _target.Character.CurrentStance += _stanceGain;
    }

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
