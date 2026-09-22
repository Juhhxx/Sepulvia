using UnityEngine;

public class VendingMachineCharm : IPassiveEffect
{
    public void OnEnterBattleEffect(BattlerController target, BattleManager battleManager) {}

    public void OnBeginTurnEffect(BattlerController[] others) {}

    public void OnEndTurnEffect(BattlerController[] others) {}

    public void OnTriggerEffect(BattlerController[] others) {}
}
