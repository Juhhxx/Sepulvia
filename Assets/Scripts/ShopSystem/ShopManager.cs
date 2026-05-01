using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int _shopSize = 4;

    [SerializeField] private Canvas _shopCanvas;
    [SerializeField] private GameObject _buyPanel;
    [SerializeField] private GameObject _sellPanel;
    [SerializeField] private GameObject _upgradesPanel;
    [SerializeField] private GameObject _soulsPanel;

    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _upgradesButton;
    [SerializeField] private Button _soulsButton;


    [SerializeField] private ShopUIManager _shopUIManager;
    [SerializeField] private InventoryUIManager _inventoryUIManager;
    [SerializeField] private List<ItemInfo> _possibleItems;

    private PlayerController _player;

    public enum ShopState
    {
        Buy,
        Sell,
        Upgrades,
        Souls
    }

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _buyButton.onClick.AddListener(() => ToggleShopPanel(ShopState.Buy));
        _sellButton.onClick.AddListener(() => ToggleShopPanel(ShopState.Sell));
        _upgradesButton.onClick.AddListener(() => ToggleShopPanel(ShopState.Upgrades));
        _soulsButton.onClick.AddListener(() => ToggleShopPanel(ShopState.Souls));

        _shopUIManager.CreateShopBuyDisplays(_shopSize);
        _shopUIManager.CreateShopSellDisplays(_player.PlayerCharacter.Inventory.MaxInventorySpaces);

        ToggleShop(true);
    }

    public void ToggleShop(bool onOff)
    {
        _shopCanvas.gameObject.SetActive(onOff);
        ToggleShopPanel(ShopState.Buy);
    }

    public void ToggleShopPanel(ShopState state)
    {
        _buyPanel.SetActive(state == ShopState.Buy);
        _sellPanel.SetActive(state == ShopState.Sell);
        _upgradesPanel.SetActive(state == ShopState.Upgrades);
        _soulsPanel.SetActive(state == ShopState.Souls);

        switch (state)
        {
            case ShopState.Buy:
                SetUpShopBuy();
                break;
            case ShopState.Sell:
                SetUpShopSell();
                break;
            case ShopState.Upgrades:
                break;
            case ShopState.Souls:
                break;
        }
    }

    private void SetUpShopBuy()
    {
        List<ItemInfo> items = new List<ItemInfo>();

        for (int i = 0; i < _shopSize; i++)
        {
            items.Add(_possibleItems[Random.Range(0, _possibleItems.Count)]);
        }

        _shopUIManager.UpdateShopBuyDisplays(items);

        _shopUIManager.GetButtonsBuy().ForEach(button => button.onClick.RemoveAllListeners());

        var buttons = _shopUIManager.GetButtonsBuy();
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            Debug.Log("Adding listener to button " + i);
            buttons[i].onClick.AddListener(() => {

                bool hasBought = BuyItem(items[index]);

                if (hasBought) buttons[index].GetComponent<ShopDisplayManager>().DoDisplayPurchaseAnim();
                else buttons[index].GetComponent<ShopDisplayManager>().DoDisplayNotEnoughAnim();

            });
        }
    }

    private void SetUpShopSell()
    {
        _shopUIManager.UpdateShopSellDisplays(_player.PlayerCharacter.Inventory);

        var buttons = _shopUIManager.GetButtonsSell();
        for (int i = 0; i < buttons.Count; i++)
        {
            ItemStack stack = 
            (i < _player.PlayerCharacter.Inventory.ItemSlots.Count) ? 
            _player.PlayerCharacter.Inventory.ItemSlots[i] : null;

            if (stack == null)
            {
                buttons[i].enabled = false;
                buttons[i].onClick.RemoveAllListeners();
                continue;
            }
            
            if (stack.Item.CanBeSold)
            {
                buttons[i].enabled = true;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    Debug.Log("Selling " + stack.Item.Name);
                    SellItem(stack);
                });
            }
            else
            {
                buttons[i].enabled = false;
                Color c = Color.white;
                c.a = 0.35f;
                buttons[i].transform.GetChild(0).GetComponent<Image>().color = c;
                buttons[i].onClick.RemoveAllListeners();
            }
        }
    }

    public bool BuyItem(ItemInfo item)
    {
        if (_player.Essence >= item.Value)
        {
            _player.ChangeEssence(-item.Value);
            _player.PlayerCharacter.Inventory.AddItem(item);

            return true;
        }
        
        return false;
    }

    public void SellItem(ItemStack stack)
    {
        _player.ChangeEssence(stack.Item.Value / 2);
        _player.PlayerCharacter.Inventory.RemoveItem(stack);
        SetUpShopSell();
    }
}
