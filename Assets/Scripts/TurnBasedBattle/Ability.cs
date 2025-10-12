using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public abstract class Ability : ScriptableObject
{
    public string AbilityName; // Display name for the UI
    //public Sprite icon;
    public int BasePower;
    public int HealthCost;
    public abstract void Execute(BattleEntities user, BattleEntities target, BattleSystem battleSystem); //this gets called from the BattleSystem when the player (or the enemy) uses this ability
    
}
