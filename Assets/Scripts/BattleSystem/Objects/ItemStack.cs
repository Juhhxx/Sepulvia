using System;
using UnityEngine;

[Serializable]
public class ItemStack
{
    [field: SerializeField] public ItemInfo Item { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }

    public void AddItem()
    {
        if (!IsFull()) Amount++;
    }
    public void RemoveItem()
    {
        if (Amount > 0) Amount--;
    }

    public bool IsFull() => Amount == Item.StackMaximum;

    public ItemStack(ItemInfo item)
    {
        // Item = item.Instantiate();
        Item = item;
        Amount = 1;
    }

    public ItemStack(ItemInfo item, int amount)
    {
        // Item = item.Instantiate();
        Item = item;
        Amount = amount;
    }
}