using Unity.VisualScripting;
using UnityEngine;

public class ExtraStance : IPassiveEffect
{
    [SerializeField] private int _forMoves;
    [SerializeField] private float _extraStance;
    private BattlerController _target;
    private PassiveEffect _effect;
    private BattleManager _battleManager;
    private int _timesUsed = 0;
    private bool _done = false;

    private void AddBonusStance(BattleAction action)
    {
        if (action.User == _target)
        {
            if (_timesUsed < _forMoves)
            {
                _timesUsed++;
                _target.Character.CurrentStance += _extraStance;
            }

            if (_timesUsed == _forMoves)
            {
                _target.Character.RemovePassiveEffect(_effect);
                _battleManager.OnActionExecuted -= AddBonusStance;
            }
        }
    }

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        _target = target;
        _effect = effect;
        _battleManager = battleManager;

        _battleManager.OnActionExecuted += AddBonusStance;

        _done = true;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
