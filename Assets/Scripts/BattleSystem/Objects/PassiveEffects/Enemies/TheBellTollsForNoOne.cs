using UnityEngine;

public class TheBellTollsForNoOne : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;
    [SerializeField] private StatusEffectInfo _enrageStatusEffect;
    private bool _done = false;

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        Debug.Log($"DOING BELL TOOL PASSIVE {_done}");

        if (_done) return;

        _newSoulBurn.OnSoulBurn += (_,_) => target.StatusEffectManager.AddStatusEffect(_enrageStatusEffect.Instantiate());

        battleManager.ChangeSoulBurn(_newSoulBurn);

        _done = true;
        Debug.Log($"DID BELL TOOL PASSIVE {_done}");
    }

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
