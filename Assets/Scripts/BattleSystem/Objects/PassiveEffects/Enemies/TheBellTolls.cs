using UnityEngine;

public class TheBellTolls : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;


    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager)
    {
        battleManager.ChangeSoulBurn(_newSoulBurn);
    }

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
