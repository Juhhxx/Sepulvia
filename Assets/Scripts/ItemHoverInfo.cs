using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemInfo _item;
    private InventoryManager _inventoryManager;
    private bool _active = false;

    public void SetUpHover(ItemInfo item, InventoryManager inventoryManager)
    {
        _item = item;
        _inventoryManager = inventoryManager;
        _active = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_active) return;
        
        _inventoryManager?.ToggleItemInfo(true, _item);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_active) return;
        _inventoryManager?.ToggleItemInfo(false);
    }
}
