using Unity.VisualScripting;
using UnityEngine;

public class InventoryResolver : MonoBehaviour
{
    public void UseItem(ItemStack stack, BattlerController user, BattlerController[] targets)
    {
        Debug.Log($"{user.Character.Name} USED {stack.Item.Name}");

        var vmc = user.Character.GetPassiveEffect<VendingMachineCharm>();

        if (vmc != null)
        {
            float rnd = UnityEngine.Random.Range(0,1f);

            if (rnd > vmc.ChanceOfNotConsuming)
            {
                user.Character.Inventory.RemoveItem(stack);
            }
        }
        else
        {
            user.Character.Inventory.RemoveItem(stack);
        }

        switch (stack.Item.Type)
        {
            case ItemTypes.Consumable:

                stack.Item.ConsumableLogic.OnConsumed(user, targets);
                break;
            
            case ItemTypes.Equippable:

                break;
            
            case ItemTypes.Save:
            
                break;

        }
    }

    public void UseItem(ItemStack stack, Character user)
    {
        Debug.Log($"{user.Name} USED {stack.Item.Name}");

        var vmc = user.GetPassiveEffect<VendingMachineCharm>();

        if (vmc != null)
        {
            float rnd = UnityEngine.Random.Range(0,1f);

            if (rnd > vmc.ChanceOfNotConsuming)
            {
                user.Inventory.RemoveItem(stack);
            }
        }
        else
        {
            user.Inventory.RemoveItem(stack);
        }

        switch (stack.Item.Type)
        {
            case ItemTypes.Consumable:

                stack.Item.ConsumableLogic.OnConsumed(user);
                break;
            
            case ItemTypes.Equippable:

                break;
            
            case ItemTypes.Save:
            
                SaveManager.Instance.SaveGame();
                break;

        }
    }

}
