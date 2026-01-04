using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;

public class InventoryEditor : MonoBehaviour
{
    [Header("Inventory Logic Parameters")]
    [Space(5f)]
    [SerializeField, InputAxis] private string _inventoryButton;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private InventoryResolver _inventoryResolver;

    [SerializeField] private GameObject _confirmUsePanel;
    [SerializeField] private GameObject _confirmEquipPanel;
    [SerializeField] private GameObject _confirmUnequipPanel;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] private GameObject _raycastBlockerPanel;

    private PlayerController _player;

    private ItemStack _selectedStack;
    private ItemInfo _selectedEquipment;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _confirmEquipPanel.SetActive(false);
        _confirmUnequipPanel.SetActive(false);
    }

    private void Update()
    {
        if (!_player.InBattle) CheckInventoryOpen();
    }

    private void CheckInventoryOpen()
    {
        if (Input.GetButtonDown(_inventoryButton))
        {
            _inventoryManager.ShowInventory(_player.PlayerCharacter.Inventory);
            SetUpButtons();
        }
    }

    private void SetUpButtons()
    {
        var tmp = _inventoryManager.GetItemButtons();

        for (int i = 0; i < tmp.Count; i++)
        {
            ItemStack stack = 
            (i < _player.PlayerCharacter.Inventory.ItemSlots.Count) ? 
            _player.PlayerCharacter.Inventory.ItemSlots[i] : null;
            
            if (stack != null)
            {
                tmp[i].enabled = true;
                tmp[i].onClick.RemoveAllListeners();
                
                if (stack.Item.Type == ItemTypes.Equippable)
                {
                    tmp[i].onClick.AddListener(() =>
                    {
                        UpdatedSelectedStack(stack);
                        _raycastBlockerPanel.SetActive(true);
                        _confirmEquipPanel.SetActive(true);
                        _confirmEquipPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"Are you sure you want to equip {stack.Item.Name}?";
                    });
                }
                else if (stack.Item.Type != ItemTypes.Save)
                {
                    tmp[i].onClick.AddListener(() =>
                    {
                        UpdatedSelectedStack(stack);
                        _raycastBlockerPanel.SetActive(true);
                        _confirmUsePanel.SetActive(true);
                        _confirmUsePanel.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"Are you sure you want to use {stack.Item.Name}?";
                    });
                }
                else
                {
                    tmp[i].onClick.AddListener(() =>
                    {
                        _raycastBlockerPanel.SetActive(true);
                        _warningPanel.SetActive(true);
                        _warningPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"Not Implemented :3";
                    });
                }
            
                tmp[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(stack.Item, _inventoryManager);
            }
        }

        tmp = _inventoryManager.GetEquipmentButtons();

        for (int i = 0; i < tmp.Count; i++)
        {
            ItemInfo item = 
            (i < _player.PlayerCharacter.Inventory.EquipmentSlots.Count) ? 
            _player.PlayerCharacter.Inventory.EquipmentSlots[i] : null;
            
            if (item != null)
            {
                tmp[i].enabled = true;
                tmp[i].onClick.RemoveAllListeners();
                tmp[i].onClick.AddListener(() =>
                {
                    UpdatedSelectedEquipment(item);
                    _raycastBlockerPanel.SetActive(true);
                    _confirmUnequipPanel.SetActive(true);
                    _confirmUnequipPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Are you sure you want to unequip {item.Name}?";
                });
            
                tmp[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(item, _inventoryManager);
            }
        }
    }

    public void UpdatedSelectedStack(ItemStack stack) => _selectedStack = stack;
    public void UpdatedSelectedEquipment(ItemInfo item) => _selectedEquipment = item;

    public void Use()
    {
        UseItem(_selectedStack);
        _inventoryManager.ShowInventory(_player.PlayerCharacter.Inventory);
        SetUpButtons();
    }

    public void Equip()
    {
        EquipItem(_selectedStack);
        _inventoryManager.ShowInventory(_player.PlayerCharacter.Inventory);
        SetUpButtons();
    }

    public void Unequip()
    {
        UnequipItem(_selectedEquipment);
        _inventoryManager.ShowInventory(_player.PlayerCharacter.Inventory);
        SetUpButtons();
    }

    public void UseItem(ItemStack stack)
    {
        if (!_player.PlayerCharacter.Inventory.Contains(stack.Item)) return;

        _inventoryResolver.UseItem(stack.Item, _player.PlayerCharacter);
        _player.PlayerCharacter.Inventory.RemoveItem(stack);
    }

    public void EquipItem(ItemStack stack)
    {
        if (!_player.PlayerCharacter.Inventory.Contains(stack.Item) || stack.Item.Type != ItemTypes.Equippable) return;

        if (_player.PlayerCharacter.Inventory.EquipmentFull())
        {
            _raycastBlockerPanel.SetActive(true);
            _warningPanel.SetActive(true);
            _warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You don't have space for more equipment!";
            return;
        }

        if (stack.Item.EquipmentType == EquipmentType.MoveModidier)
        {
            if (_player.PlayerCharacter.Inventory.HasEquiped(stack.Item))
            {
                _raycastBlockerPanel.SetActive(true);
                _warningPanel.SetActive(true);
                _warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You can't equip any more of this soul!";
                return;
            }
        }

        _player.PlayerCharacter.Inventory.AddEquipment(stack.Item);
        _player.PlayerCharacter.Inventory.RemoveItem(stack);
        _player.PlayerCharacter.CheckEquipment();
    }

    public void UnequipItem(ItemInfo item)
    {
        if (_player.PlayerCharacter.Inventory.IsFull())
        {
            _raycastBlockerPanel.SetActive(true);
            _warningPanel.SetActive(true);
            _warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Your inventory is full!";
            return;
        }

        if (!_player.PlayerCharacter.Inventory.HasEquiped(item)) return;

        bool canAdd = _player.PlayerCharacter.Inventory.AddItem(item);

        if (canAdd)
        {
            _player.PlayerCharacter.Inventory.RemoveEquipment(item);
            _player.PlayerCharacter.ResetMoves();
        }
        else
        {
            _raycastBlockerPanel.SetActive(true);
            _warningPanel.SetActive(true);
            _warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Your inventory is full!";
        }
    }

}
