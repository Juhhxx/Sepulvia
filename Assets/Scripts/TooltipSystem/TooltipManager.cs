using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviourSingleton<TooltipManager>
{
    [SerializeField] private Tooltip _tooltip;

    private Canvas _canvas;
    private HorizontalLayoutGroup _layoutGroup;

    private void Awake()
    {
        base.SingletonCheck(this, false, true);

        _canvas = GetComponentInChildren<Canvas>();
        _layoutGroup = _tooltip.GetComponent<HorizontalLayoutGroup>();
    }

    private void Start()
    {
        Hide();
    }

    public void Show(TooltipInfo info)
    {
        _tooltip?.ToggleTooltip(true, info);
    }

    public void Hide()
    {
        _tooltip?.ToggleTooltip(false);
    }

    public void PositionTooltip(Vector2 position, TextAnchor alignment)
    {
        Vector2 pos = position;

        _tooltip.RectTransform.anchoredPosition = pos;

        _layoutGroup.childAlignment = alignment;
    }

    public void PositionTooltip(RectTransform rectTrans, Vector2 offset, TextAnchor alignment)
    {
        Vector3 worldPosition = rectTrans.TransformPoint(offset);

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(
            rectTrans.GetComponentInParent<Canvas>().worldCamera,
            worldPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            screenPosition,
            _canvas.worldCamera,
            out Vector2 localPosition
        );

        _tooltip.RectTransform.anchoredPosition = localPosition;

        _layoutGroup.childAlignment = alignment;
    }
}

[Serializable]
public class TooltipInfo
{
    public string Header;
    public string Content;
    public TooltipInfo[] AdditionalInfo;

    public TooltipInfo (string header, string content, params TooltipInfo[] additional)
    {
        Header = header;
        Content = content;
        AdditionalInfo = additional;
    }
}
