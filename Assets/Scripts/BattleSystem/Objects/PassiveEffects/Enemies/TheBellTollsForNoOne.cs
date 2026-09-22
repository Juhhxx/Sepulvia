using UnityEngine;

public class TheBellTollsForNoOne : IPassiveEffect
{
    [SerializeField] private SoulBurnProfile _newSoulBurn;


    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager)
    {
        _newSoulBurn.OnSoulBurn += (_,_) => target.Character.AddModifier(new (Stats.PullStrength, 1));

        battleManager.ChangeSoulBurn(_newSoulBurn);
    }

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
