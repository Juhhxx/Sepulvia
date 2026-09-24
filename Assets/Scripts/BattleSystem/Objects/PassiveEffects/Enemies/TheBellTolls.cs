using UnityEngine;

public class TheBellTolls : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        battleManager.ChangeSoulBurn(_newSoulBurn);

        _done = false;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
