using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private Canvas _shopCanvas;

    [SerializeField] private Transform _shopBuyDisplayParent;
    [SerializeField] private Transform _shopSellDisplayParent;
    [SerializeField] private Transform _shopUpgradesDisplayParent;
    [SerializeField] private GameObject _shopBuyDisplayPrefab;
    [SerializeField] private GameObject _shopSellDisplayPrefab;
    [SerializeField] private GameObject _shopUpgradesDisplayPrefab;
    [SerializeField] private Transform _shopItemAnimFinalPosition;
    [SerializeField] private GameObject _shopItemAnimPrefab;

    [SerializeField] private GameObject _buyPanel;
    [SerializeField] private GameObject _sellPanel;
    [SerializeField] private GameObject _upgradesPanel;
    [SerializeField] private GameObject _soulsPanel;

    public Action<ShopManager.ShopState> OnShopPanelToggle;

    public void ToggleShop(bool onOff)
    {
        if (onOff)
        {
            PauseManager.Instance.Pause();
            ToggleShopPanel(ShopManager.ShopState.Buy);
        }
        else
        {
            PauseManager.Instance.UnPause();
        }
        

        _shopCanvas.gameObject.SetActive(onOff);
    }

    public void ToggleShopPanel(ShopManager.ShopState state)
    {
        _buyPanel.SetActive(state == ShopManager.ShopState.Buy);
        _sellPanel.SetActive(state == ShopManager.ShopState.Sell);
        _upgradesPanel.SetActive(state == ShopManager.ShopState.Upgrades);
        _soulsPanel.SetActive(state == ShopManager.ShopState.Souls);

        OnShopPanelToggle?.Invoke(state);
    }

    // Buy Displays
    private List<ShopDisplayManager> _shopBuyDisplays = new List<ShopDisplayManager>();
    private List<Button> _buttonsBuy = new List<Button>();
    public List<Button> GetButtonsBuy() => _buttonsBuy;

    public void CreateShopBuyDisplays(int shopSize)
    {
        for (int i = 0; i < shopSize; i++)
        {
            GameObject display = Instantiate(_shopBuyDisplayPrefab, _shopBuyDisplayParent);
            _shopBuyDisplays.Add(display.GetComponent<ShopDisplayManager>());
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

    // Sell Displays
    private List<InventorySlotManager> _shopSellDisplays = new List<InventorySlotManager>();
    private List<Button> _buttonsSell = new List<Button>();
    public List<Button> GetButtonsSell() => _buttonsSell;

    public void CreateShopSellDisplays(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject display = Instantiate(_shopSellDisplayPrefab, _shopSellDisplayParent);
            _shopSellDisplays.Add(display.GetComponent<InventorySlotManager>());
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

    // Upgrade Displays
    private List<ShopUpgradeManager> _upgradeDisplays = new List<ShopUpgradeManager>();
    private List<Button> _buttonsUpgrades = new List<Button>();
    public List<Button> GetButtonsUpgrades() => _buttonsUpgrades;

    public void CreateShopUpgradeDisplays(int upgradeAmount)
    {
        for (int i = 0; i < upgradeAmount; i++)
        {
            GameObject display = Instantiate(_shopUpgradesDisplayPrefab, _shopUpgradesDisplayParent);

            _upgradeDisplays.Add(display.GetComponent<ShopUpgradeManager>());
        }
    }
    public void UpdateShopUpgradeDisplays(List<ShopManager.Upgrade> upgrades, Character character)
    {
        _buttonsUpgrades.Clear();

        for (int i = 0; i < upgrades.Count; i++)
        {
            ShopManager.Upgrade upgrade = upgrades[i];

            ShopUpgradeManager display = _upgradeDisplays[i];

            (int statLevel, int statValue) = character.GetStatLevelValue(upgrade.Stat);

            display.SetUpDisplay(upgrade.Stat.ToTitle(), statLevel, statValue, upgrade.Icon);

            _buttonsUpgrades.Add(display.GetComponent<Button>());
        }
    }


    public void SpawnItemAnim(Sprite sprite, Vector3 spawnPosition)
    {
        GameObject anim = Instantiate(_shopItemAnimPrefab, _shopCanvas.transform);
        anim.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
        anim.GetComponent<ShopItemAnimation>().DoItemAnim(sprite, _shopItemAnimFinalPosition.position);
    }

}
