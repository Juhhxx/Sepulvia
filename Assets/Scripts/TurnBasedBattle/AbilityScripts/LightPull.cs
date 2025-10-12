using UnityEngine;

[CreateAssetMenu(fileName = "LightPull", menuName = "Scriptable Objects/LightPull")]
public class LightPull : Ability
{
    public override void Execute(BattleEntities user, BattleEntities target, BattleSystem battleSystem) //this gets called from the BattleSystem when the player (or the enemy) uses this ability
    {
        target.CurrHealth -= 5;
    }
}
