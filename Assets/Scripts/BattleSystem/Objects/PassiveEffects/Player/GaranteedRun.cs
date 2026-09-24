using UnityEngine;

public class GaranteedRun : IPassiveEffect
{
    private float _originalValue;
    private Player _target;
    private PassiveEffect _effect;
    private BattleManager _battleManager;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        _target = target.Character as Player;
        _effect = effect;
        _battleManager = battleManager; 
        _originalValue = _target.RunChance;

        _target.RunChance = 1;

        _battleManager.OnBattleEnd += ResetValue;

        _done = true;
    }

    private void ResetValue()
    {
        _target.RunChance = _originalValue;
        _target.RemovePassiveEffect(_effect);

        _battleManager.OnBattleEnd -= ResetValue;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
