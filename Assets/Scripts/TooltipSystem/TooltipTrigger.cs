using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    public TooltipInfo[] additional;
    public bool _useDefaultValues = false;

    [SerializeField] private bool _useRelativePosition = true;
    [SerializeField, ShowIf(nameof(_useRelativePosition))] private Vector2 _tooltipOffset;
    [SerializeField, HideIf(nameof(_useRelativePosition))] private Vector2 _tooltipPosition;
    [SerializeField] private TooltipPosition _tooltipAlignment = TooltipPosition.TopLeft;

    private RectTransform _rectTrans;
    private ITooltipable _tooltipObject;

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _tooltipObject = GetComponent<ITooltipable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var info = _useDefaultValues ? new TooltipInfo(header, content, additional)
                                        : _tooltipObject.GetTooltipInfo();

        TooltipManager.Instance.Show(info);

        Vector2 pos = _useRelativePosition ? _rectTrans.anchoredPosition + _tooltipOffset
                                            : _tooltipPosition;

        if (!_useRelativePosition) TooltipManager.Instance.PositionTooltip(pos, _tooltipAlignment);
        else TooltipManager.Instance.PositionTooltip(_rectTrans, _tooltipOffset, _tooltipAlignment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }

    public void OnDisable()
    {
        TooltipManager.Instance.Hide();
    }
}
