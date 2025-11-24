using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;

    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _inventorySlots;

    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _itemInfoPanelRect;
    [SerializeField] private Vector2 _mouseOffset = new Vector2(25f, -25f);
    [SerializeField] private Canvas _canvas;

    private List<GameObject> _createdObjects;

    private List<Button> _buttons;
    public List<Button> GetInventoryButtons() => _buttons;

    private void Update()
    {
        FollowMouse();
    }

    public void ShowInventory(InventoryInfo inventory)
    {
        if (_createdObjects != null) ClearSlots();
        else _createdObjects = new List<GameObject>();

        _buttons = new List<Button>();

        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < inventory.ItemSlots.Count) ? inventory.ItemSlots[i] : null;

            GameObject slot = Instantiate(_inventorySlotPrefab, _inventorySlots.transform);
            Image image = slot.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI tmp = slot.GetComponentInChildren<TextMeshProUGUI>();

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

        _inventoryCanvas.SetActive(true);
    }

    public void HideInventory()
    {
        _inventoryCanvas.SetActive(false);
        _itemInfoPanel.SetActive(false);
    }

    private void ClearSlots()
    {
        foreach(GameObject go in _createdObjects) DestroyImmediate(go);
        _createdObjects.Clear();
    }

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
        canvasPos += _mouseOffset; 
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
