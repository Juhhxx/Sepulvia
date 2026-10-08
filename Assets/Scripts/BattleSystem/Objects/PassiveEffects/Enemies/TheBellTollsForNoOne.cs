using UnityEngine;

public class TheBellTollsForNoOne : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        Debug.Log($"DOING BELL TOOL PASSIVE {_done}");

        if (_done) return;

        _newSoulBurn.OnSoulBurn += (_,_) => target.Character.AddModifier(new (Stats.PullStrength, 1));

        battleManager.ChangeSoulBurn(_newSoulBurn);

        _done = true;
        Debug.Log($"DID BELL TOOL PASSIVE {_done}");
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
