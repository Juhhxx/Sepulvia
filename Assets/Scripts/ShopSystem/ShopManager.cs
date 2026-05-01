using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int _shopSize = 4;

    [SerializeField] private Canvas _shopCanvas;

    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _upgradesButton;
    [SerializeField] private Button _soulsButton;


    [SerializeField] private ShopUIManager _shopUIManager;
    [SerializeField] private List<ItemInfo> _possibleItems;
    private List<ItemInfo> _shopItems = new List<ItemInfo>();

    private PlayerController _player;

    public enum ShopState
    {
        Buy,
        Sell,
        Upgrades,
        Souls
    }

    [Serializable]
    public struct Upgrade
    {
        public Stats Stat;
        public Sprite Icon;
    }
    [SerializeField] private List<Upgrade> _possibleUpgrades;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _buyButton.onClick.AddListener(() => _shopUIManager.ToggleShopPanel(ShopState.Buy));
        _sellButton.onClick.AddListener(() => _shopUIManager.ToggleShopPanel(ShopState.Sell));
        _upgradesButton.onClick.AddListener(() => _shopUIManager.ToggleShopPanel(ShopState.Upgrades));
        _soulsButton.onClick.AddListener(() => _shopUIManager.ToggleShopPanel(ShopState.Souls));

        _shopUIManager.CreateShopBuyDisplays(_shopSize);
        _shopUIManager.CreateShopSellDisplays(_player.PlayerCharacter.Inventory.MaxInventorySpaces);
        _shopUIManager.CreateShopUpgradeDisplays(_possibleUpgrades.Count);

        _shopUIManager.ToggleShop(false);

        _shopUIManager.OnShopPanelToggle += (ShopState state) => {
            switch (state)
            {
                case ShopState.Buy:
                    SetUpShopBuy();
                    break;
                case ShopState.Sell:
                    SetUpShopSell();
                    break;
                case ShopState.Upgrades:
                    SetUpShopUpgrades();
                    break;
                case ShopState.Souls:
                    break;
            }
        };
    }

    public void SetUpShop()
    {
        SetUpShopBuy();
        SetUpShopSell();
        SetUpShopUpgrades();
    }

    public void ChooseShopItems()
    {
        // For now, just randomly choose items. Later can implement some sort of item progression based on player progression or something
        _shopItems.Clear();

        for (int i = 0; i < _shopSize; i++)
        {
            int rnd = UnityEngine.Random.Range(0, _possibleItems.Count);
            _shopItems.Add(_possibleItems[rnd]);
        }
    }

    private void SetUpShopBuy()
    {
        _shopUIManager.UpdateShopBuyDisplays(_shopItems);

        _shopUIManager.GetButtonsBuy().ForEach(button => button.onClick.RemoveAllListeners());

        var buttons = _shopUIManager.GetButtonsBuy();
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            Debug.Log("Adding listener to button " + i);
            buttons[i].onClick.AddListener(() => {

                bool hasBought = BuyItem(_shopItems[index]);

                if (hasBought)
                {
                    buttons[index].GetComponent<ShopDisplayManager>().DoDisplayPurchaseAnim();
                    _shopUIManager.SpawnItemAnim(_shopItems[index].Sprite, ScreenToCanvas(Input.mousePosition));
                }
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

    private void SetUpShopUpgrades()
    {
        _shopUIManager.UpdateShopUpgradeDisplays(_possibleUpgrades, _player.PlayerCharacter);

        var buttons = _shopUIManager.GetButtonsUpgrades();
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;

            if (index < _possibleUpgrades.Count)
            {
                Upgrade upgrade = _possibleUpgrades[index];

                (int cost, int amountGained) = _player.PlayerCharacter.LevelUpCost(upgrade.Stat);

                ShopUpgradeManager display = buttons[i].GetComponent<ShopUpgradeManager>();

                buttons[i].enabled = true;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    bool hasBought = BuyUpgrade(upgrade.Stat);

                    if (hasBought)
                    {
                        display.DoDisplayPurchaseAnim();
                    }
                    else
                    {
                        display.DoDisplayNotEnoughAnim();
                    }
                });

                PointerButtonEvents pointerEvents = buttons[i].GetComponent<PointerButtonEvents>();

                (int statLevel, int statValue) = _player.PlayerCharacter.GetStatLevelValue(upgrade.Stat);

                pointerEvents.OnPointerEnterEvent.RemoveAllListeners();
                pointerEvents.OnPointerEnterEvent.AddListener(() =>
                {
                    display.ShowUpgrade(cost, amountGained, statLevel, statValue);
                    display.DoDisplaySelectAnim();
                });

                pointerEvents.OnPointerExitEvent.RemoveAllListeners();
                pointerEvents.OnPointerExitEvent.AddListener(() =>
                {
                    display.UpdateDisplay(statLevel, statValue);
                    display.DoDisplayDeselectAnim();
                    display.ResetSelection();
                });

                if (pointerEvents.IsPointerOver)
                {
                    display.ShowUpgrade(cost, amountGained, statLevel, statValue);
                }

            }
            else
            {
                buttons[i].enabled = false;
                buttons[i].onClick.RemoveAllListeners();
            }
        }
    }


    public bool BuyItem(ItemInfo item)
    {
        if (_player.Essence >= item.Value && !_player.PlayerCharacter.Inventory.IsFull())
        {
            _player.ChangeEssence(-item.Value);
            _player.PlayerCharacter.Inventory.AddItem(item);

            return true;
        }
        
        return false;
    }

    public void SellItem(ItemStack stack)
    {
        _player.ChangeEssence(stack.Item.Value / 2); // Selling gives back half the value
        _player.PlayerCharacter.Inventory.RemoveItem(stack);
        SetUpShopSell();
    }

    public bool BuyUpgrade(Stats stat)
    {
        (int cost, _) = _player.PlayerCharacter.LevelUpCost(stat);

        if (_player.Essence >= cost)
        {
            _player.ChangeEssence(-cost);
            _player.PlayerCharacter.LevelUpStat(stat);

            SetUpShopUpgrades();

            return true;
        }
        return false;
    }

    private Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        RectTransform canvasRect = _shopCanvas.GetComponent<RectTransform>();
        Camera _uiCamera = _shopCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            _uiCamera, 
            out Vector2 localPoint
        );

        return localPoint;
    }
}
