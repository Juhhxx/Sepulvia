using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private Transform _shopBuyDisplayParent;
    [SerializeField] private Transform _shopSellDisplayParent;
    [SerializeField] private GameObject _shopBuyDisplayPrefab;
    [SerializeField] private GameObject _shopSellDisplayPrefab;

    private List<ShopDisplayManager> _shopBuyDisplays = new List<ShopDisplayManager>();
    private List<InventorySlotManager> _shopSellDisplays = new List<InventorySlotManager>();

    private List<Button> _buttonsBuy = new List<Button>();
    public List<Button> GetButtonsBuy() => _buttonsBuy;

    private List<Button> _buttonsSell = new List<Button>();
    public List<Button> GetButtonsSell() => _buttonsSell;

    public void CreateShopBuyDisplays(int shopSize)
    {
        for (int i = 0; i < shopSize; i++)
        {
            GameObject display = Instantiate(_shopBuyDisplayPrefab, _shopBuyDisplayParent);
            _shopBuyDisplays.Add(display.GetComponent<ShopDisplayManager>());
        }
    }

    public void CreateShopSellDisplays(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject display = Instantiate(_shopSellDisplayPrefab, _shopSellDisplayParent);
            _shopSellDisplays.Add(display.GetComponent<InventorySlotManager>());
        }
    }

    public void UpdateShopBuyDisplays(List<ItemInfo> items)
    {
        _buttonsBuy.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            ItemInfo item = items[i];

            ShopDisplayManager display = _shopBuyDisplays[i];

            display.UpdateDisplay(item.Sprite, item.Value);

            _buttonsBuy.Add(display.GetComponent<Button>());
        }
    }

    public void UpdateShopSellDisplays(Inventory inventory)
    {
        _buttonsSell.Clear();
        
        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < inventory.ItemSlots.Count) ? inventory.ItemSlots[i] : null;
            InventorySlotManager display = _shopSellDisplays[i].GetComponent<InventorySlotManager>();

            if (stack != null) display.UpdateSlot(stack.Item.Sprite, stack.Amount);
            else display.UpdateSlot();

            _buttonsSell.Add(display.GetComponent<Button>());
        }
    }

}
