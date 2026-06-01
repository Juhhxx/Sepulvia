using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : RandomBehaviour
{
    [SerializeField] private int _shopSize = 4;

    [SerializeField] private Canvas _shopCanvas;

    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _upgradesButton;
    [SerializeField] private Button _soulsButton;


    [SerializeField] private Button _rerollButton;
    [SerializeField] private int _rerollPrice;


    [SerializeField] private ShopUIManager _shopUIManager;
    [SerializeField] private ShopSoulUpgradeManager _shopSoulUpgradeManager;

    [SerializeField] private List<ItemInfo> _possibleItems;
    private List<ItemInfo> _shopItems = new List<ItemInfo>();

    private PlayerController _player;
    private PlayerOverworldUI _overworldUI;

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
        TryInitializeRandom();
        
        _player = FindAnyObjectByType<PlayerController>();
        _overworldUI = FindAnyObjectByType<PlayerOverworldUI>();

        SetUpButtons();
        SetUpShopBuyReroll();

        _shopSoulUpgradeManager.SetUp(_player.PlayerCharacter.Inventory);
        _shopSoulUpgradeManager.OnSelectDeselectSouls += ToggleItemInfoPanel;

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
                    _shopSoulUpgradeManager.CreateAvailableSouls(_player.PlayerCharacter.Inventory);
                    _shopSoulUpgradeManager.SetUpAvailableSouls();
                    _shopSoulUpgradeManager.SetUpSoulButtons();
                    break;
            }
        };

        _shopSoulUpgradeManager.OnUpgradeDone += () => _shopUIManager.ToggleItemInfo(false);
    }

    public void ToggleShop(bool onOff)
    {
        _shopUIManager.ToggleShop(onOff);
        _player.InShop = onOff;
        _overworldUI.ToggleEquipmentDash(!onOff);
    }

    private void SetUpButtons()
    {
        _buyButton.onClick.AddListener(() =>
        {
            _shopUIManager.ToggleShopPanel(ShopState.Buy);
            _shopUIManager.DoButtonClickAnim(_buyButton);
        });
        _buyButton.GetComponent<PointerButtonEvents>().OnPointerEnterEvent.AddListener(() => _shopUIManager.DoButtonSelectAnim(_buyButton));
        _buyButton.GetComponent<PointerButtonEvents>().OnPointerExitEvent.AddListener(() => _shopUIManager.DoButtonDeselectAnim(_buyButton));

        _sellButton.onClick.AddListener(() => {
            _shopUIManager.ToggleShopPanel(ShopState.Sell);
            _shopUIManager.DoButtonClickAnim(_sellButton);
        });
        _sellButton.GetComponent<PointerButtonEvents>().OnPointerEnterEvent.AddListener(() => _shopUIManager.DoButtonSelectAnim(_sellButton));
        _sellButton.GetComponent<PointerButtonEvents>().OnPointerExitEvent.AddListener(() => _shopUIManager.DoButtonDeselectAnim(_sellButton));


        _upgradesButton.onClick.AddListener(() => {
            _shopUIManager.ToggleShopPanel(ShopState.Upgrades);
            _shopUIManager.DoButtonClickAnim(_upgradesButton);
        });
        _upgradesButton.GetComponent<PointerButtonEvents>().OnPointerEnterEvent.AddListener(() => _shopUIManager.DoButtonSelectAnim(_upgradesButton));
        _upgradesButton.GetComponent<PointerButtonEvents>().OnPointerExitEvent.AddListener(() => _shopUIManager.DoButtonDeselectAnim(_upgradesButton));

        _soulsButton.onClick.AddListener(() => {
            _shopUIManager.ToggleShopPanel(ShopState.Souls);
            _shopUIManager.DoButtonClickAnim(_soulsButton);
        });
        _soulsButton.GetComponent<PointerButtonEvents>().OnPointerEnterEvent.AddListener(() => _shopUIManager.DoButtonSelectAnim(_soulsButton));
        _soulsButton.GetComponent<PointerButtonEvents>().OnPointerExitEvent.AddListener(() => _shopUIManager.DoButtonDeselectAnim(_soulsButton));
    }

    private void Update()
    {
        Vector2 canvasPos = ScreenToCanvas(Input.mousePosition);
        _shopUIManager.MoveItemInfoPanel(canvasPos);
    }

    private void ToggleItemInfoPanel(bool onOff, ItemInfo item = null)
    {
        _shopUIManager.ToggleItemInfo(onOff, item);
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
            int rnd = _random.Next(0, _possibleItems.Count);
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
            buttons[i].onClick.AddListener(() => {

                bool hasBought = BuyItem(_shopItems[index]);

                if (hasBought)
                {
                    buttons[index].GetComponent<ShopDisplayManager>().DoDisplayPurchaseAnim();
                    _shopUIManager.SpawnItemAnim(_shopItems[index].Sprite, ScreenToCanvas(Input.mousePosition));
                }
                else buttons[index].GetComponent<ShopDisplayManager>().DoDisplayNotEnoughAnim();
            });

            PointerButtonEvents pointerEvents = buttons[i].GetComponent<PointerButtonEvents>();

            pointerEvents.OnPointerEnterEvent.RemoveAllListeners();
            pointerEvents.OnPointerEnterEvent.AddListener(() => ToggleItemInfoPanel(true, _shopItems[index]));

            pointerEvents.OnPointerExitEvent.RemoveAllListeners();
            pointerEvents.OnPointerExitEvent.AddListener(() => ToggleItemInfoPanel(false));
        }
    }

    private void SetUpShopBuyReroll()
    {
        _rerollButton.GetComponent<ShopRerollUIManager>().UpdateDisplay(_rerollPrice);
        _rerollButton.onClick.RemoveAllListeners();
        _rerollButton.onClick.AddListener(() =>
        {
            bool hasBought = BuyReroll();

            if (hasBought)
            {
                _rerollButton.GetComponent<ShopRerollUIManager>().DoPurchaseAnim();
            }
            else _rerollButton.GetComponent<ShopRerollUIManager>().DoNotEnoughAnim();
        });
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

            ItemHoverInfo itemHoverInfo = buttons[i].GetComponent<ItemHoverInfo>();

            if (stack == null)
            {
                buttons[i].enabled = false;
                buttons[i].onClick.RemoveAllListeners();
                itemHoverInfo.SetUpHover(_shopUIManager.ToggleItemInfo);
                continue;
            }
            
            if (stack.Item.CanBeSold)
            {
                buttons[i].enabled = true;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    SellItem(stack);
                });
            }
            else
            {
                buttons[i].enabled = false;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].GetComponent<InventorySlotManager>().SetDisabled();
            }

            itemHoverInfo.SetUpHover(stack.Item, _shopUIManager.ToggleItemInfo);
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
        if (_player.Essence >= item.Value && _player.PlayerCharacter.Inventory.AddItem(item))
        {
            _player.ChangeEssence(-item.Value);

            return true;
        }
        
        return false;
    }

    public bool BuyReroll()
    {
        if (_player.Essence >= _rerollPrice)
        {
            _player.ChangeEssence(-_rerollPrice);
            ChooseShopItems();
            SetUpShopBuy();

            return true;
        }
        
        return false;
    }

    public void SellItem(ItemStack stack)
    {
        _player.ChangeEssence(stack.Item.Value / 2); // Selling gives back half the value
        if (stack.Amount == 1) _shopUIManager.ToggleItemInfo(false);
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
