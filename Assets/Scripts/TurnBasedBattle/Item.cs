using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string ItemName;  // Display name for UI in inventory for example
    [System.Serializable]
    public class AbilitySwap //Defines how an item modifies a speciific ability
    {
        public Ability BaseAbility;  //The original neutral form
        public Ability ModifiedAbility;  //The transformed version associated to this item
    }

    public AbilitySwap[] abilitySwaps;

    public Ability GetModifiedAbility(Ability baseAbility)  //Given a base ability, return the modified version (if one exists for this item). To be called in the BattleSystem
    {                                                        // When updating each characetr before battle
        foreach (AbilitySwap swap in abilitySwaps)
        {
            if (swap.BaseAbility == baseAbility)
            {
                return swap.ModifiedAbility;
            }
        }
        return baseAbility; //default to neutral ability if no swap exists
    }
}
