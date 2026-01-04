using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "InventoryInfo", menuName = "Inventory/New Inventory")]
public class InventoryInfo : ScriptableObject
{
    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField] public int MaxInventorySpaces { get; private set; }
    [field: SerializeField] public List<ItemStack> ItemSlots { get; private set; }

    public bool IsFull()
    {
        if (ItemSlots.Count < MaxInventorySpaces) return false;

        foreach (ItemStack stack in ItemSlots)
        {
            if (!stack.IsFull())
            {
                return false;
            }
        }

        return true;
    }

    public bool Contains(ItemInfo item)
    {
        if (ItemSlots.Count == 0) return false;

        foreach (ItemStack stack in ItemSlots)
        {
            if (stack.Item == item) return true;
        }

        return false;
    }

    public bool AddItem(ItemInfo item)
    {
        if (IsFull()) return false;

        foreach (ItemStack stack in ItemSlots)
        {
            if (item == stack.Item)
            {
                if (!stack.IsFull())
                {
                    stack.AddItem();
                    return true;
                }
                else continue;
            }
        }

        if (ItemSlots.Count < MaxInventorySpaces)
            ItemSlots.Add(new ItemStack(item));
        else
            return false;

        return true;
    }

    public void RemoveItem(ItemInfo item)
    {
        if (!Contains(item)) return;

        var reverse = new List<ItemStack>(ItemSlots);
        reverse.Reverse();

        foreach(ItemStack stack in reverse)
        {
            if (stack.Item == item)
            {
                stack.RemoveItem();

                if (stack.Amount == 0)
                {
                    ItemSlots.Remove(stack);
                }
                return;
            }
        }
    }

    // Remove Item from Specific Stack
    public void RemoveItem(ItemStack stack)
    {
        if (!ItemSlots.Contains(stack)) return;

        stack.RemoveItem();

        if (stack.Amount == 0)
        {
            ItemSlots.Remove(stack);
        }
    }

    [field: Space(10)]
    [field: Header("Character Equipment")]
    [field: Space(5)]
    [field: SerializeField] public int MaxEquipmentSpaces { get; private set; }

    [field: OnValueChanged("CheckEquipment")]
    [field: SerializeField, Expandable] public List<ItemInfo> EquipmentSlots { get; private set; }

    private void CheckEquipment()
    {
        EquipmentSlots.RemoveAll((e) => e.Type != ItemTypes.Equippable);
    }

    public bool EquipmentFull()
    {
        return EquipmentSlots.Count >= MaxEquipmentSpaces;
    }

    public bool HasEquiped(ItemInfo item)
    {
        return EquipmentSlots.Contains(item);
    }

    public bool AddEquipment(ItemInfo item)
    {
        if (item.Type != ItemTypes.Equippable) return false;

        if (EquipmentSlots.Count == MaxEquipmentSpaces) return false;

        EquipmentSlots.Add(item);

        return true;
    }

    public void RemoveEquipment(ItemInfo item)
    {
        if (item.Type != ItemTypes.Equippable) return;

        if (!EquipmentSlots.Contains(item)) return;

        EquipmentSlots.Remove(item);
    }

    public InventoryInfo Instantiate()
    {
        InventoryInfo inv = Instantiate(this);

        inv.CheckEquipment();

        // for (int i = 0; i < inv.Items.Count; i++)
        // {
        //     var tmp = inv.Items[i];

        //     inv.Items[i] = new ItemStack(tmp.Item.Instantiate(), tmp.Amount);
        // }

        return inv;
    }
}
