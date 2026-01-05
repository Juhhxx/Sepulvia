using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Space(10f)]
    [Header("Inventory UI Parameters")]
    [Space(5f)]
    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _inventorySlots;
    [SerializeField] private GameObject _equipmentSlots;

    [SerializeField] private PlayerStatsUI _statsUI;

    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _itemInfoPanelRect;
    [SerializeField] private Canvas _canvas;

    private List<GameObject> _createdObjects;

    private List<Button> _buttons;

    public List<Button> GetAllButtons() => _buttons;
    public List<Button> GetItemButtons() => _buttons.GetRange(0, _inventory.MaxInventorySpaces);
    public List<Button> GetEquipmentButtons() => _buttons.GetRange(_inventory.MaxInventorySpaces, _inventory.MaxEquipmentSpaces);

    private InventoryInfo _inventory;
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
    public void ShowInventory(InventoryInfo inventory)
    {
        _statsUI.UpdateStats(_player.PlayerCharacter);
        
        _inventory = inventory;

        if (_createdObjects != null) ClearSlots();
        else _createdObjects = new List<GameObject>();

        _buttons = new List<Button>();

        ShowItemSpaces(inventory);
        ShowEquipmentSpaces(inventory);

        _inventoryCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    private void ShowItemSpaces(InventoryInfo inventory)
    {
        GameObject inventorySlotPrefab = _inventorySlots.transform.GetChild(0).gameObject;

        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < inventory.ItemSlots.Count) ? inventory.ItemSlots[i] : null;

            GameObject slot = Instantiate(inventorySlotPrefab, _inventorySlots.transform);
            Image image = slot.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI tmp = slot.GetComponentInChildren<TextMeshProUGUI>();

            slot.SetActive(true);

            if (stack != null)
            {
                image.sprite = stack.Item.Sprite;
                image.color = Color.white;
                
                tmp.text = $"{stack.Amount}";
            }
            else
            {
                image.color = new Color(1f , 1f, 1f, 0f);
                tmp.text = "";
            }
            
            _createdObjects.Add(slot);
            _buttons.Add(slot.GetComponent<Button>());
        }
    }

    private void ShowEquipmentSpaces(InventoryInfo inventory)
    {
        GameObject equipmentSlotPrefab = _equipmentSlots.transform.GetChild(0).gameObject;
        
        for (int i = 0; i < inventory.MaxEquipmentSpaces; i++)
        {
            ItemInfo item = (i < inventory.EquipmentSlots.Count) ? inventory.EquipmentSlots[i] : null;

            GameObject slot = Instantiate(equipmentSlotPrefab, _equipmentSlots.transform);
            Image image = slot.transform.GetChild(0).GetComponent<Image>();

            slot.SetActive(true);

            if (item != null)
            {
                image.sprite = item.Sprite;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(1f , 1f, 1f, 0f);
            }
            
            _createdObjects.Add(slot);
            _buttons.Add(slot.GetComponent<Button>());
        }
    }

    public void HideInventory()
    {
        _inventoryCanvas.SetActive(false);
        _itemInfoPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ClearSlots()
    {
        foreach(GameObject go in _createdObjects) DestroyImmediate(go);
        _createdObjects.Clear();
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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            _uiCamera, 
            out Vector2 localPoint
        );

        return localPoint;
    }

}
