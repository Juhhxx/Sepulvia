using UnityEngine;

public class InventoryResolver : MonoBehaviour
{
    public void UseItem(ItemInfo item, CharacterInfo user)
    {
        Debug.Log($"{user.Name} USED {item.Name}");

        switch (item.Type)
        {
            case ItemTypes.Immediate:

                ApplyItemEffect(item, user);
                break;
            
            case ItemTypes.LongTerm:

                user.AddModifier(item.Modifier.Instantiate());
                break;
        }
    }

    private void ApplyItemEffect (ItemInfo item, CharacterInfo user)
    {
        switch (item.Stat)
        {
            case Stats.Stance:

                user.CurrentStance += item.Amount;

                int realAmount = item.Amount > user.MaxStance ? user.MaxStance : item.Amount;

                DialogueManager.Instance?.AddDialogue($"{user.Name} recovered {realAmount} stance.");
                break;
            
            default:
                
                break;
        }
    }
}
