using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemInfo _item;
    private InventoryUIManager _inventoryUIManager;
    private bool _active = false;

    public void SetUpHover(ItemInfo item, InventoryUIManager inventoryManager)
    {
        _item = item;
        _inventoryUIManager = inventoryManager;
        _active = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_active) return;
        
        _inventoryUIManager?.ToggleItemInfo(true, _item);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_active) return;
        _inventoryUIManager?.ToggleItemInfo(false);
    }
}
