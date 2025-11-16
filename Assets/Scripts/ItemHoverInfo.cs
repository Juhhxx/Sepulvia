using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemInfo _item;
    private InventoryManager _inventoryManager;
    private Button _button;
    private bool _active = false;

    public void SetUpHover(ItemInfo item, InventoryManager inventoryManager)
    {
        _item = item;
        _inventoryManager = inventoryManager;
        _button = GetComponent<Button>();
        _active = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_active) return;
        if (!_button.enabled) return;
        
        _inventoryManager?.ToogleItemInfo(true, _item);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_active) return;
        _inventoryManager?.ToogleItemInfo(false);
    }
}
