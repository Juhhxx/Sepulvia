using UnityEngine;

public class PhotoOfTwoFriends : IPassiveEffect
{
    [SerializeField] private float _stanceGain;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        target.Character.CurrentStance += _stanceGain;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}

    public void ResetPassiveEffect() {}
}
