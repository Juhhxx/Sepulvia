using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    public List<TooltipInfo> additional;

    [SerializeField] private bool _useRelativePosition = true;
    [SerializeField, ShowIf(nameof(_useRelativePosition))] private Vector2 _tooltipOffset;
    [SerializeField, HideIf(nameof(_useRelativePosition))] private Vector2 _tooltipPosition;

    private RectTransform _rectTrans;

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.Show(new TooltipInfo(header, content), additional);

        Vector2 pos = _useRelativePosition ? _rectTrans.anchoredPosition + _tooltipOffset
                                            : _tooltipPosition;

        if (!_useRelativePosition) TooltipManager.Instance.PositionTooltip(pos);
        else TooltipManager.Instance.PositionTooltip(_rectTrans, _tooltipOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}
