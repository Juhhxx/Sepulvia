using UnityEngine;

public class TheBellTollsForNoOne : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        if (_done) return;

        _newSoulBurn.OnSoulBurn += (_,_) => target.Character.AddModifier(new (Stats.PullStrength, 1));

        battleManager.ChangeSoulBurn(_newSoulBurn);

        _done = true;
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
