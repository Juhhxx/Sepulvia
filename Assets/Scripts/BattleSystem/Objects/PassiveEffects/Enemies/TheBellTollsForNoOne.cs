using UnityEngine;

public class TheBellTollsForNoOne : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;
    [SerializeField] private StatusEffectInfo _enrageStatusEffect;

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        _newSoulBurn.OnSoulBurn += (_,_) => target.StatusEffectManager.AddStatusEffect(_enrageStatusEffect.Instantiate());

        battleManager.ChangeSoulBurn(_newSoulBurn);
    }

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}

    public void ResetPassiveEffect() {}
}
