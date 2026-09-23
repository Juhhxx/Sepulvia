using Unity.VisualScripting;
using UnityEngine;

public class InventoryResolver : MonoBehaviour
{
    public void UseItem(ItemInfo item, BattlerController user, BattlerController[] targets)
    {
        Debug.Log($"{user.Character.Name} USED {item.Name}");

        switch (item.Type)
        {
            case ItemTypes.Consumable:

                item.ConsumableLogic.OnConsumed(user, targets);
                break;
            
            case ItemTypes.Equippable:

                break;
            
            case ItemTypes.Save:
            
                break;

        }
    }

    public void UseItem(ItemInfo item, Character user)
    {
        Debug.Log($"{user.Name} USED {item.Name}");

        switch (item.Type)
        {
            case ItemTypes.Consumable:

                item.ConsumableLogic.OnConsumed(user);
                break;
            
            case ItemTypes.Equippable:

                break;
            
            case ItemTypes.Save:
            
                SaveManager.Instance.SaveGame();
                break;

        }
    }

}
