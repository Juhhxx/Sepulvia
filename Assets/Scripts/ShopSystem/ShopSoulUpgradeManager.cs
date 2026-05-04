using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShopSoulUpgradeManager : MonoBehaviour
{
    [SerializeField] private ShopSoulUpgradeUIManager _shopSoulUpgradeUIManager;

    private ItemInfo[] _soulsForUpgrade = new ItemInfo[2];
    public event Action OnChangeSouls;
    public event Action<bool, ItemInfo> OnSelectDeselectSouls;
    public event Action OnUpgradeDone;

    private Inventory _playerInventory;
    [SerializeField] private Inventory _temporaryInventory;

    public void SetUp(Inventory playerInventory)
    {
        _playerInventory = playerInventory;

        OnChangeSouls += () => {
            Debug.Log("SOULS CHANGED");
            _shopSoulUpgradeUIManager.UpdateSoulUpgradeSlots(_soulsForUpgrade);
            UpdateAvailableSouls();
            SetUpAvailableSouls();
            CheckUpgrade();
        };
    }

    public void CreateAvailableSouls(Inventory inventory)
    {
        _temporaryInventory = new Inventory(10, 0);

        for (int i = 0; i < _soulsForUpgrade.Length; i++) _soulsForUpgrade[i] = null;
        _shopSoulUpgradeUIManager.UpdateSoulUpgradeSlots(_soulsForUpgrade);

        var availableSouls = inventory.ItemSlots
            .FindAll(itemStack => itemStack.Item.Type == ItemTypes.Equippable)
            .Select(stack => new ItemStack(stack.Item, stack.Amount)).ToList();

        foreach (ItemStack stack in availableSouls) for (int i = 0; i < stack.Amount; i++) _temporaryInventory.AddItem(stack.Item);

        _shopSoulUpgradeUIManager.CreateAvailableSouls(_temporaryInventory.ItemSlots.Count);
        CheckUpgrade();
    }
    public void UpdateAvailableSouls()
    {
        _shopSoulUpgradeUIManager.CreateAvailableSouls(_temporaryInventory.ItemSlots.Count);
    }
    public void SetUpAvailableSouls()
    {
        _shopSoulUpgradeUIManager.UpdateShopAvailableSouls(_temporaryInventory.ItemSlots.ToArray());

        var buttons = _shopSoulUpgradeUIManager.GetButtonsSouls();
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;

            buttons[i].onClick.AddListener(() =>
            {
                AddSoul(_temporaryInventory.ItemSlots[index].Item);
            });

            ItemHoverInfo itemHoverInfo = buttons[i].GetComponent<ItemHoverInfo>();

            itemHoverInfo.SetUpHover(_temporaryInventory.ItemSlots[index].Item, OnSelectDeselectSouls);
        }
    }

    public void SetUpSoulButtons()
    {
        _shopSoulUpgradeUIManager.SoulOneButton.onClick.RemoveAllListeners();
        _shopSoulUpgradeUIManager.SoulOneButton.onClick.AddListener(() =>
        {
            RemoveSoul(0);
            UpdateAvailableSouls();
            SetUpAvailableSouls();
        });

        PointerButtonEvents soulOnePointerEvents = _shopSoulUpgradeUIManager.SoulOneButton.GetComponent<PointerButtonEvents>();
        soulOnePointerEvents.OnPointerEnterEvent.RemoveAllListeners();
        soulOnePointerEvents.OnPointerEnterEvent.AddListener(() =>
        {
            if (_soulsForUpgrade[0] != null) OnSelectDeselectSouls?.Invoke(true, _soulsForUpgrade[0]);
        });
        soulOnePointerEvents.OnPointerExitEvent.RemoveAllListeners();
        soulOnePointerEvents.OnPointerExitEvent.AddListener(() =>
        {
            if (_soulsForUpgrade[0] != null) OnSelectDeselectSouls?.Invoke(false, null);
        });


        _shopSoulUpgradeUIManager.SoulTwoButton.onClick.RemoveAllListeners();
        _shopSoulUpgradeUIManager.SoulTwoButton.onClick.AddListener(() =>
        {
            RemoveSoul(1);
            UpdateAvailableSouls();
            SetUpAvailableSouls();
        });

        PointerButtonEvents soulTwoPointerEvents = _shopSoulUpgradeUIManager.SoulTwoButton.GetComponent<PointerButtonEvents>();
        soulTwoPointerEvents.OnPointerEnterEvent.RemoveAllListeners();
        soulTwoPointerEvents.OnPointerEnterEvent.AddListener(() => 
        {
            if (_soulsForUpgrade[1] != null) OnSelectDeselectSouls?.Invoke(true, _soulsForUpgrade[1]);
        });
        soulTwoPointerEvents.OnPointerExitEvent.RemoveAllListeners();
        soulTwoPointerEvents.OnPointerExitEvent.AddListener(() =>
        {
            if (_soulsForUpgrade[1] != null) OnSelectDeselectSouls?.Invoke(false, null);
        });
    }

    private bool AddSoul(ItemInfo soul)
    {
        if (_soulsForUpgrade[0] == null)
        {
            _soulsForUpgrade[0] = soul;
            _temporaryInventory.RemoveItem(soul);

            OnChangeSouls?.Invoke();
            return true;
        }
        else if (_soulsForUpgrade[1] == null)
        {
            _soulsForUpgrade[1] = soul;
            _temporaryInventory.RemoveItem(soul);

            OnChangeSouls?.Invoke();
            return true;
        }

        return false;
    }

    private void RemoveSoul(int index)
    {
        if (index >= 0 && index < _soulsForUpgrade.Length)
        {
            _temporaryInventory.AddItem(_soulsForUpgrade[index]);
            _soulsForUpgrade[index] = null;

            OnChangeSouls?.Invoke();
        }
    }

    private void CheckUpgrade()
    {
        Debug.Log("CHEKING UPGRADES");

        PointerButtonEvents soulResultEvents = _shopSoulUpgradeUIManager.SoulUpgradeResultButton.GetComponent<PointerButtonEvents>();
        ItemHoverInfo itemHoverInfo = _shopSoulUpgradeUIManager.SoulUpgradeResultButton.GetComponent<ItemHoverInfo>();

        if (_soulsForUpgrade[0] != null && _soulsForUpgrade[1] != null)
        {
            if (_soulsForUpgrade[0] == _soulsForUpgrade[1])
            {
                if (_soulsForUpgrade[0].Upgrade != null)
                {
                    Debug.Log("UPGRADE SUCCESSFUL");
                    _shopSoulUpgradeUIManager.UpdateSoulUpgradeResultSlot(_soulsForUpgrade[0].Upgrade);

                    _shopSoulUpgradeUIManager.SoulUpgradeResultButton.onClick.RemoveAllListeners();
                    _shopSoulUpgradeUIManager.SoulUpgradeResultButton.onClick.AddListener(() =>
                    {
                        _playerInventory.AddItem(_soulsForUpgrade[0].Upgrade);
                        _playerInventory.RemoveItem(_soulsForUpgrade[0]);
                        _playerInventory.RemoveItem(_soulsForUpgrade[1]);

                        for (int i = 0; i < _soulsForUpgrade.Length; i++) _soulsForUpgrade[i] = null;
                        
                        CreateAvailableSouls(_playerInventory);
                        OnChangeSouls?.Invoke();
                        OnUpgradeDone?.Invoke();
                    });

                    itemHoverInfo.SetUpHover(_soulsForUpgrade[0].Upgrade, OnSelectDeselectSouls);

                    return;
                }
                else
                {
                    _shopSoulUpgradeUIManager.UpdateWarningText("Cannot be upgraded further.");
                }
            }
            else
            {
                _shopSoulUpgradeUIManager.UpdateWarningText("Incompatible souls.");
            }
        }
        else
        {
            _shopSoulUpgradeUIManager.UpdateWarningText("");
        }

        _shopSoulUpgradeUIManager.SoulUpgradeResultButton.onClick.RemoveAllListeners();
        _shopSoulUpgradeUIManager.SoulUpgradeResultButton.onClick.RemoveAllListeners();
        _shopSoulUpgradeUIManager.UpdateSoulUpgradeResultSlot();

        itemHoverInfo.SetUpHover(OnSelectDeselectSouls);
    }

}
