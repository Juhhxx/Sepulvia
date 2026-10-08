using System.Collections.Generic;
using System.ComponentModel;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUIManager : MonoBehaviour
{
    [Space(10f)]
    [Header("Inventory UI Parameters")]
    [Space(5f)]
    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _itemSlots;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private GameObject _equipmentSlots;
    [SerializeField] private GameObject _battleEquipmentSlots;
    [SerializeField] private GameObject _utilityEquipmentSlots;
    [SerializeField] private GameObject _altMoveEquipmentSlots;
    [SerializeField] private GameObject _equipmentSlotPrefab;

    [SerializeField] private PlayerStatsUI _statsUI;

    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _itemInfoPanelRect;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private List<Button> _buttons;
    [SerializeField] private List<Button> _CombatEquipmentButtons;
    [SerializeField] private List<Button> _UtilityEquipmentButtons;
    [SerializeField] private List<Button> _AltMoveEquipmentButtons;

    public List<Button> GetAllButtons() => _buttons;
    public List<Button> GetCombatEquipmentButtons() => _CombatEquipmentButtons;
    public List<Button> GetUtilityEquipmentButtons() => _UtilityEquipmentButtons;
    public List<Button> GetAltMoveEquipmentButtons() => _AltMoveEquipmentButtons;
    public List<Button> GetItemButtons() => _buttons.GetRange(0, _inventory.MaxInventorySpaces);
    public List<Button> GetEquipmentButtons() => _buttons.GetRange(_inventory.MaxInventorySpaces, _inventory.MaxEquipmentSpaces);

    private Inventory _inventory;
    private PlayerController _player;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        FollowMouse();
    }

    // Generic Inventory Functionality
    public void CreateInventory(Inventory inventory)
    {
        _buttons = new List<Button>();

        _inventory = inventory;

        CreateItemSpaces(_inventory);
        CreateEquipmentSpaces(_inventory);
    }

    private bool _inventoryOpen = false;
    public void ResetInventory()
    {
        _inventoryOpen = false;
    }
    public bool ShowInventory()
    {
        if (_inventoryOpen)
        {
            HideInventory();
            return false;
        }

        _statsUI.UpdateStats(_player.PlayerCharacter);
        
        ShowItemSpaces(_inventory);
        ShowEquipmentSpaces(_inventory);

        _inventoryCanvas.SetActive(true);

        PauseManager.Instance.Pause();

        _inventoryOpen = true;

        return true;
    }

    public void UpdateInventory()
    {
        _statsUI.UpdateStats(_player.PlayerCharacter);
        
        ShowItemSpaces(_inventory);
        ShowEquipmentSpaces(_inventory);
    }
    
    public void HideInventory()
    {
        _inventoryCanvas.SetActive(false);
        _itemInfoPanel.SetActive(false);
        PauseManager.Instance.UnPause();

        _inventoryOpen = false;
    }

    List<GameObject> _inventoryItemSlots = new List<GameObject>();
    private void CreateItemSpaces(Inventory inventory)
    {
        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            GameObject slot = Instantiate(_itemSlotPrefab, _itemSlots.transform);
            slot.SetActive(true);

            _inventoryItemSlots.Add(slot);
            _buttons.Add(slot.GetComponent<Button>());
        }
    }

    private void ShowItemSpaces(Inventory inventory)
    {
        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < inventory.ItemSlots.Count) ? inventory.ItemSlots[i] : null;
            InventorySlotManager slot = _inventoryItemSlots[i].GetComponent<InventorySlotManager>();

            if (stack != null) slot.UpdateSlot(stack);
            else slot.UpdateSlot();
        }
    }

    [SerializeField] List<GameObject> _CombatEquipmentSlots;
    [SerializeField] List<GameObject> _UtilityEquipmentSlots;
    [SerializeField] List<GameObject> _AltMoveEquipmentSlots;
    [SerializeField] List<GameObject> _inventoryEquipmentSlots;
    private void CreateEquipmentSpaces(Inventory inventory)
    {
        // Enable combat slots and add them to the total equipment slots
        for(int i = 0; i < inventory.MaxCombatEquipmentAmount; i++)
        {
           GameObject inventorySlot = _CombatEquipmentSlots[i];

            inventorySlot.gameObject.SetActive(true);
            _inventoryEquipmentSlots.Add(inventorySlot);
            _buttons.Add(inventorySlot.GetComponent<Button>());
        }

        // Enable utility slots and add them to the total equipment slots
        for(int i = 0; i < inventory.MaxUtilityEquipmentAmount; i++)
        {
           GameObject inventorySlot = _UtilityEquipmentSlots[i];

            inventorySlot.gameObject.SetActive(true);
            _inventoryEquipmentSlots.Add(inventorySlot);
            _buttons.Add(inventorySlot.GetComponent<Button>());
        }

        // Enable alt move slots and add them to the total equipment slots
        for(int i = 0; i < inventory.MaxAltMoveEquipmentAmount; i++)
        {  
           GameObject inventorySlot = _AltMoveEquipmentSlots[i];

            inventorySlot.gameObject.SetActive(true);
            _inventoryEquipmentSlots.Add(inventorySlot);
            _buttons.Add(inventorySlot.GetComponent<Button>());
        }

        /*
        for (int i = 0; i < inventory.MaxEquipmentSpaces; i++)
        {
            GameObject slot = Instantiate(_equipmentSlotPrefab, _equipmentSlots.transform);
            slot.SetActive(true);

            _inventoryEquipmentlots.Add(slot);
            _buttons.Add(slot.GetComponent<Button>());
        }
        */
    }

    private void ShowEquipmentSpaces(Inventory inventory)
    {
        ShowCombatEquipmentSpaces(inventory);
        ShowUtilityEquipmentSpaces(inventory);
        ShowAltMoveEquipmentSpaces(inventory);
    }

    private void ShowCombatEquipmentSpaces(Inventory inventory)
    {   
        for (int i = 0; i < inventory.MaxCombatEquipmentAmount; i++)
        {
            ItemInfo item = (i < inventory.CombatEquipmentSlots.Count) ? inventory.CombatEquipmentSlots[i] : null;

            InventorySlotManager slot;

            slot = _CombatEquipmentSlots[i].GetComponent<InventorySlotManager>();

            if (item != null) slot.UpdateSlot(item.Sprite);
            else slot.UpdateSlot();            
        }
    }

    private void ShowUtilityEquipmentSpaces(Inventory inventory)
    {   
        for (int i = 0; i < inventory.MaxUtilityEquipmentAmount; i++)
        {
            ItemInfo item = (i < inventory.UtilityEquipmentSlots.Count) ? inventory.UtilityEquipmentSlots[i] : null;

            InventorySlotManager slot;

            slot = _UtilityEquipmentSlots[i].GetComponent<InventorySlotManager>();

            if (item != null) slot.UpdateSlot(item.Sprite);
            else slot.UpdateSlot();            
        }
    }

    private void ShowAltMoveEquipmentSpaces(Inventory inventory)
    {   
        for (int i = 0; i < inventory.MaxAltMoveEquipmentAmount; i++)
        {
            ItemInfo item = (i < inventory.AltMoveEquipmentSlots.Count) ? inventory.AltMoveEquipmentSlots[i] : null;

            InventorySlotManager slot;

            slot = _AltMoveEquipmentSlots[i].GetComponent<InventorySlotManager>();

            if (item != null) slot.UpdateSlot(item.Sprite);
            else slot.UpdateSlot();            
        }
    }    

    /*
    private List<GameObject> FindEquipmentTypeList(EquippableTypes equipmentType)
    {   
        List<GameObject> result = null;
        switch(equipmentType) {
            case EquippableTypes.Combat:
                result = _CombatEquipmentSlots;
                break;
            case EquippableTypes.Utility:
                result = _UtilityEquipmentSlots;
                break;
            case EquippableTypes.AltMove:
                result = _AltMoveEquipmentSlots;
                break;                             
        }

        return result;
    }
    */

    private void ClearSlots()
    {
        foreach(GameObject go in _inventoryItemSlots) Destroy(go);
        _inventoryItemSlots.Clear();

        foreach(GameObject go in _inventoryEquipmentSlots) Destroy(go);
        _inventoryEquipmentSlots.Clear();
    }


    // Info Panel
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

    private void FollowMouse()
    {
        Vector2 canvasPos = ScreenToCanvas(Input.mousePosition);
        _itemInfoPanelRect.anchoredPosition = canvasPos;
    }

    private Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        _uiCamera = _canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            _uiCamera, 
            out Vector2 localPoint
        );

        return localPoint;
    }

}
