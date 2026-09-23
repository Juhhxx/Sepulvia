using UnityEngine;

public class GaranteedRun: IPassiveEffect
{
    private float _originalValue;
    private Player _target;
    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        _target = target.Character as Player;
        _originalValue = _target.RunChance;

        _target.RunChance = 1;

        battleManager.OnBattleEnd += () =>
        {
            _target.RunChance = _originalValue;
            _target.RemovePassiveEffect(effect);
        };
    }

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
