using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemInfo _item;
    private bool _active = false;
    public event Action<bool, ItemInfo> OnItemHover;

    public void SetUpHover(ItemInfo item, Action<bool, ItemInfo> toggle)
    {
        _item = item;

        OnItemHover = toggle;
        _active = true;
    }

    public void SetUpHover(Action<bool, ItemInfo> toggle)
    {
        OnItemHover = toggle;
        _active = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_active)
        {
            OnItemHover?.Invoke(false, null);
            return;
        }
        
        OnItemHover?.Invoke(true, _item);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnItemHover?.Invoke(false, null);
    }
}
