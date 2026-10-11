using UnityEngine;

public class TheBellTolls : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;

    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect)
    {
        battleManager.ChangeSoulBurn(_newSoulBurn);
    }

    public void OnBeginTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnEndTurnEffect(BattlerController target, BattleManager battleManager, PassiveEffect effect) {}

    public void OnTriggerEffect(BattlerController[] others) {}

    public void ResetPassiveEffect() {}
}
