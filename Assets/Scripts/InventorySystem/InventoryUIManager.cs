using System.Collections.Generic;
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
    [SerializeField] private GameObject _equipmentSlotPrefab;

    [SerializeField] private PlayerStatsUI _statsUI;

    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _itemInfoPanelRect;
    [SerializeField] private Canvas _canvas;

    private List<Button> _buttons;

    public List<Button> GetAllButtons() => _buttons;
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

            if (stack != null) slot.UpdateSlot(stack.Item.Sprite, stack.Amount);
            else slot.UpdateSlot();
        }
    }

    List<GameObject> _inventoryEquipmentlots = new List<GameObject>();
    private void CreateEquipmentSpaces(Inventory inventory)
    {
        for (int i = 0; i < inventory.MaxEquipmentSpaces; i++)
        {
            GameObject slot = Instantiate(_equipmentSlotPrefab, _equipmentSlots.transform);
            slot.SetActive(true);

            _inventoryEquipmentlots.Add(slot);
            _buttons.Add(slot.GetComponent<Button>());
        }
    }

    private void ShowEquipmentSpaces(Inventory inventory)
    {
        for (int i = 0; i < inventory.MaxEquipmentSpaces; i++)
        {
            ItemInfo item = (i < inventory.EquipmentSlots.Count) ? inventory.EquipmentSlots[i] : null;
            InventorySlotManager slot = _inventoryEquipmentlots[i].GetComponent<InventorySlotManager>();

            if (item != null) slot.UpdateSlot(item.Sprite);
            else slot.UpdateSlot();
        }
    }

    private void ClearSlots()
    {
        foreach(GameObject go in _inventoryItemSlots) Destroy(go);
        _inventoryItemSlots.Clear();

        foreach(GameObject go in _inventoryEquipmentlots) Destroy(go);
        _inventoryEquipmentlots.Clear();
    }


    // Info Panel
    public void ToggleItemInfo(bool onOff, ItemInfo item = null)
    {
        if (onOff)
        {
            _panelTitle.text = $"{item.Name}";
            _panelDescription.text = $"{item.Description}";
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
