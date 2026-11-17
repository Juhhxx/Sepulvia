using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Settings")]
    [SerializeField] private bool _constantUpdate = false;
    [ShowIf(nameof(_useDefaultValues))]
    [SerializeField] private string header;
    [ShowIf(nameof(_useDefaultValues))]
    [SerializeField] private string content;
    [ShowIf(nameof(_useDefaultValues))]
    [SerializeField] private TooltipInfo[] additional;

    [SerializeField] private bool _useDefaultValues = false;

    [Header("Tooltip Position Settings")]
    [SerializeField] private bool _useRelativePosition = true;
    [SerializeField, ShowIf(nameof(_useRelativePosition))] private Vector2 _tooltipOffset;
    [SerializeField, HideIf(nameof(_useRelativePosition))] private Vector2 _tooltipPosition;
    [SerializeField] private TooltipPosition _tooltipAlignment = TooltipPosition.TopLeft;

    [Header("Tooltip Visual Settings")]
    [SerializeField] private float _tooltipDelay = 0.25f;

    [Header("Tooltip Debug Settings")]
    [SerializeField] private bool _showAlways = false;

    private RectTransform _rectTrans;
    private ITooltipable _tooltipObject;

    private bool _show = false;
    public bool Show
    {
        get => _show;
        set
        {
            _show = value;
        }
    }

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _tooltipObject = GetComponent<ITooltipable>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _showTooltip = StartCoroutine(ShowTooltipCR(_tooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
        Show = false;
    }

    public void OnDisable()
    {
        if (Show)
        {
            HideTooltip();
            Show = false;
        }
    }

    private Coroutine _showTooltip = null;
    private IEnumerator ShowTooltipCR(float delay)
    {
        yield return new WaitForSeconds(delay);

        ShowTooltip();
        Show = true;
    }

    private void ShowTooltip()
    {
        // Sending the tooltip info to the TooltipManager
        var info = _useDefaultValues ? new TooltipInfo(header, content, additional)
                                        : _tooltipObject.GetTooltipInfo();

        if (info == null) return;

        TooltipManager.Instance?.Show(info);

        // Positioning the tooltip based on the settings
        Vector2 pos = _useRelativePosition ? _rectTrans.anchoredPosition + _tooltipOffset
                                            : _tooltipPosition;

        if (!_useRelativePosition) TooltipManager.Instance?.PositionTooltip(pos, _tooltipAlignment);
        else TooltipManager.Instance?.PositionTooltip(_rectTrans, _tooltipOffset, _tooltipAlignment);
    }

    private void HideTooltip()
    {
        if (_showTooltip != null)
        {
            StopCoroutine(_showTooltip);
        }

        _showTooltip = null;

        TooltipManager.Instance?.Hide();
    }

    private void Update()
    {
        if ((Show && _constantUpdate) || _showAlways)
        {
            ShowTooltip();
        }
    }
}
