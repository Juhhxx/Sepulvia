using System;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviourSingleton<TooltipManager>
{
    [SerializeField] private Tooltip _tooltip;

    private Canvas _canvas;

    private void Awake()
    {
        base.SingletonCheck(this, false, true);

        _canvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        Hide();
    }

    public void Show(TooltipInfo info, List<TooltipInfo> additionalInfos = null)
    {
        _tooltip.ToggleTooltip(true, info, additionalInfos);
    }

    public void Hide()
    {
        _tooltip.ToggleTooltip(false);
    }

    public void PositionTooltip(Vector2 position)
    {
        Vector2 pos = position;

        _tooltip.RectTransform.anchoredPosition = pos;
    }

    public void PositionTooltip(RectTransform rectTrans, Vector2 offset)
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
    }
}

[Serializable]
public class TooltipInfo
{
    public string Header;
    public string Content;

    public TooltipInfo (string header, string content)
    {
        Header = header;
        Content = content;
    }
}
