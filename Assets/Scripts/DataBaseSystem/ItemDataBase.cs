using UnityEngine;

public class ItemDataBase : DataBase<ItemInfo>
{
    public ItemInfo GetRandomBuyableItem(System.Random random)
    {
        var buyableItems = _entries.FindAll(item => item.CanBeBought);

        if (buyableItems.Count == 0)
            return null;

        return buyableItems[random.Next(0, buyableItems.Count)];
    }

    public ItemInfo GetRandomItemOfType(System.Random random, ItemTypes type)
    {
        var itemsOfType = _entries.FindAll(item => item.Type == type);

        if (itemsOfType.Count == 0)
            return null;

        return itemsOfType[random.Next(0, itemsOfType.Count)];
    }
}
