using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private Canvas _shopCanvas;
    [SerializeField] private RectTransform _shopParent;
    [SerializeField] private Image _shopBackground;

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
    private GameObject _currentPanel;

    [SerializeField] private GameObject _buySelectionImage;
    [SerializeField] private GameObject _sellSelectionImage;
    [SerializeField] private GameObject _upgradesSelectionImage;
    [SerializeField] private GameObject _soulsSelectionImage;
    private GameObject _currentSelectionImage;


    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    public Action<ShopManager.ShopState> OnShopPanelToggle;

    private void Start()
    {
        _buyPanel.SetActive(false);
        _sellPanel.SetActive(false);
        _upgradesPanel.SetActive(false);
        _soulsPanel.SetActive(false);
    }

    public void ToggleShop(bool onOff)
    {
        if (onOff)
        {
            PauseManager.Instance.Pause();
            ToggleShopPanel(ShopManager.ShopState.Buy);
            DoShopOpenAnim();
        }
        else
        {
            DoShopCloseAnim(() =>
            {
                PauseManager.Instance.UnPause();
                _shopCanvas.gameObject.SetActive(false);
            });
        }
    }

    public void ToggleShopPanel(ShopManager.ShopState state)
    {
        _currentPanel?.SetActive(false);
        _currentSelectionImage?.SetActive(false);

        switch (state)
        {
            case ShopManager.ShopState.Buy:
                _currentPanel = _buyPanel;
                _currentSelectionImage = _buySelectionImage;
                break;

            case ShopManager.ShopState.Sell:
                _currentPanel = _sellPanel;
                _currentSelectionImage = _sellSelectionImage;
                break;

            case ShopManager.ShopState.Upgrades:
                _currentPanel = _upgradesPanel;
                _currentSelectionImage = _upgradesSelectionImage;
                break;

            case ShopManager.ShopState.Souls:
                _currentPanel = _soulsPanel;
                _currentSelectionImage = _soulsSelectionImage;
                break;
        }

        _currentPanel.SetActive(true);
        _currentSelectionImage.SetActive(true);
        DoPanelOpenAnim(_currentPanel);

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
            InventorySlotManager display = _shopSellDisplays[i];

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

    public void ToggleItemInfo(bool onOff, ItemInfo item = null)
    {
        if (onOff)
        {
            _panelTitle.text = $"{item.Name}";
            _panelDescription.text = $"{item.Description}";

            if (item.CanBeSold) _panelDescription.text += $"\n\n<color=#11F227FF>Sell Value: {item.Value / 2} Essence</color>";
        }
        
        _itemInfoPanel.SetActive(onOff);
    }
    public void MoveItemInfoPanel(Vector3 position)
    {
        _itemInfoPanel.GetComponent<RectTransform>().anchoredPosition = position;
    }

    // Animations
    public void DoShopOpenAnim()
    {
        Vector3 pos = _shopParent.anchoredPosition;
        pos.y = 1920 / 2;
        _shopParent.anchoredPosition = pos;

        Color c = _shopBackground.color;
        c.a = 1;
        
        _shopCanvas.gameObject.SetActive(true);

        _shopBackground.DOColor(c, 1f);
        _shopParent.DOAnchorPos(Vector3.zero, 1f).SetEase(Ease.OutElastic);
    }
    public void DoShopCloseAnim(Action action)
    {
        Vector3 pos = _shopParent.anchoredPosition;
        pos.y = 1920 / 2;

        Color c = _shopBackground.color;
        c.a = 0;

        _shopBackground.DOColor(c, 1f);
        _shopParent.DOAnchorPos(pos, 1f).SetEase(Ease.OutElastic).OnComplete(() => action());
    }

    public void DoPanelOpenAnim(GameObject panel)
    {
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public void DoPanelCloseAnim(GameObject panel, Action onComplete = null)
    {
        panel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => onComplete());
    }

    public void DoButtonClickAnim(Button button)
    {
        button.transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            button.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutSine);
        });
    }
    public void DoButtonSelectAnim(Button button)
    {
        button.transform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.Linear);
    }
    public void DoButtonDeselectAnim(Button button)
    {
        button.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
    }
}
