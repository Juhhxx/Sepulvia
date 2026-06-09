using UnityEngine;
using System.Collections.Generic;

public class ItemDataBase : DataBase<ItemInfo>
{
    public ItemInfo GetRandomBuyableItem(System.Random random)
    {
        var buyableItems = _entries.FindAll(item => item.CanBeBought);

        if (buyableItems.Count == 0)
            return null;

        return GetItemWeighted(buyableItems, random);
    }

    public ItemInfo GetRandomItemOfType(System.Random random, ItemTypes type)
    {
        var itemsOfType = _entries.FindAll(item => item.Type == type);

        if (itemsOfType.Count == 0)
            return null;

        return GetItemWeighted(itemsOfType, random);
    }

    private ItemInfo GetItemWeighted(List<ItemInfo> weightedEntries, System.Random random)
    {
        ItemInfo selectedItem = null;

        float totalWeight = 0;
        foreach (ItemInfo item in weightedEntries)
        {
            totalWeight += 1f / item.Level;
        }

        float randomWeight = random.Next() * totalWeight;
        float cumulativeWeight = 0;

        foreach (ItemInfo item in weightedEntries)
        {
            cumulativeWeight += 1f / item.Level;
            
            if (randomWeight <= cumulativeWeight)
            {
                selectedItem = item;
                break;
            }
        }

        return selectedItem;
    }
}
