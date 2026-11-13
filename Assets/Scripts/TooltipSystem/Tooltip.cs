using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [Header("Tooltip Layout Settings")]
    [SerializeField] private RectTransform _mainTooltipRectTrans;
    [SerializeField] private TextMeshProUGUI _headerTMP;
    [SerializeField] private TextMeshProUGUI _contentTMP;
    [SerializeField] private HorizontalLayoutGroup _tooltipsLayoutGroup;

    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private float _characterWrapLimit;

    [Header("Tooltip Pivot Settings")]
    [SerializeField] private RectTransform _additionalLeftPivot;
    [SerializeField] private RectTransform _additionalRightPivot;
    [SerializeField] private List<RectTransform> _tooltipPivots = new List<RectTransform>();

    [Header("Tooltip Additional Info Settings")]
    [SerializeField] private Transform _additionalParent;
    [SerializeField] private GameObject _additionalPrefab;

    [Header("Tooltip Animation Settings")]
    [SerializeField] private float _showTooltipDuration = 0.25f;


    private RectTransform _rectTrans;
    public RectTransform RectTransform => _rectTrans;
    private RectTransform _canvasRectTrans;

    [SerializeField] private List<GameObject> _additionalTooltips = new List<GameObject>();

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _canvasRectTrans = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    // Main Tooltip
    public void ToggleTooltip(bool show, TooltipInfo info = null)
    {
        if (!show)
        {
            ClearAdditionalTooltips();
            gameObject.SetActive(show);
        }
        else
        {
            gameObject.SetActive(show);
            SetText(info);
            // DoShowTooltipAnim(); Animation looks funky
        }

    }

    private void SetText(TooltipInfo info)
    {
        if (string.IsNullOrEmpty(info.Header))
        {
            _headerTMP.gameObject.SetActive(false);
        }
        else
        {
            _headerTMP.gameObject.SetActive(true);
        }

        _headerTMP.text     = info.Header;
        _contentTMP.text    = info.Content;

        float headerLength    = _headerTMP.preferredWidth;
        float contentLength   = _contentTMP.preferredWidth;

        _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit);

        ShowAdditionalTooltips(info.AdditionalInfo);
    }

    // Additional Tooltips
    private void ShowAdditionalTooltips(TooltipInfo[] infos)
    {
        if (infos.Length <= 0) return;
        if (_additionalTooltips.Count > 0) ClearAdditionalTooltips();

        foreach (TooltipInfo info in infos)
        {
            var tt = Instantiate(_additionalPrefab, _additionalParent);
            Debug.Log($"Instantiated additional tooltip: {tt.name}");

            _additionalTooltips.Add(tt);
            
            Tooltip tooltip = tt.GetComponent<Tooltip>();

            tooltip.SetText(info);
        }

        Canvas.ForceUpdateCanvases();

        // Rebuild the vertical layout first.
        RectTransform additionalRect = _additionalParent.GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(additionalRect);

        // Then rebuild the horizontal layout containing the main tooltip.
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTrans);
    }

    private void ClearAdditionalTooltips()
    {
        foreach (GameObject tt in _additionalTooltips)
        {
            if (tt != null)
            {
                Destroy(tt);
                Debug.Log($"Destroyed additional tooltip: {tt.name}");
            }
            
        }
        _additionalTooltips.Clear();
    }

    private void FlipAdditionalTooltips(TooltipPosition alignment)
    {
        bool condition =    alignment == TooltipPosition.TopRight ||
                            alignment == TooltipPosition.MiddleRight ||
                            alignment == TooltipPosition.BottomRight;
        
        _tooltipsLayoutGroup.reverseArrangement = condition;
    }
    // Position Tooltip
    public void PositionTooltip(Vector2 position, TooltipPosition alignment)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTrans);

        var offset = GetAlignmentOffset(alignment);

        Vector2 pos = position - offset;

        _rectTrans.anchoredPosition = pos;

        FlipAdditionalTooltips(alignment);
        DoEdgeDetection(pos, alignment);
    }

    private void DoEdgeDetection(Vector2 pos, TooltipPosition alignment)
    {
        var tooltipSize = _mainTooltipRectTrans.sizeDelta;
        Debug.Log($"Tooltip size: {tooltipSize}, Canvas size: {_canvasRectTrans.rect.size}, Tooltip position: {pos}");
        Debug.Log($"Aligment points: MR {GetAlignmentPoint(TooltipPosition.MiddleRight)}, ML {GetAlignmentPoint(TooltipPosition.MiddleLeft)}, TC {GetAlignmentPoint(TooltipPosition.TopCenter)}, BC {GetAlignmentPoint(TooltipPosition.BottomCenter)}");

        var rightPivot = _additionalTooltips.Count > 0 && !_tooltipsLayoutGroup.reverseArrangement ?
                        GetAlignmentPoint(_additionalRightPivot) :
                        GetAlignmentPoint(TooltipPosition.MiddleRight);

        var leftPivot = _additionalTooltips.Count > 0 && _tooltipsLayoutGroup.reverseArrangement ?
                        GetAlignmentPoint(_additionalLeftPivot) :
                        GetAlignmentPoint(TooltipPosition.MiddleLeft);

        if (rightPivot.x > _canvasRectTrans.rect.width/2)
        {
            Debug.Log($"Tooltip going off on the right.");

            pos.x -= rightPivot.x - _canvasRectTrans.rect.width/2;
            
            _rectTrans.anchoredPosition = pos;
        }
        else if (leftPivot.x < -_canvasRectTrans.rect.width/2)
        {
            Debug.Log($"Tooltip going off on the left.");

            pos.x += -_canvasRectTrans.rect.width/2 - leftPivot.x;
            _rectTrans.anchoredPosition = pos;
        }
        else if (GetAlignmentPoint(TooltipPosition.TopCenter).y > _canvasRectTrans.rect.height/2)
        {
            Debug.Log($"Tooltip going off on the top.");

            pos.y -= GetAlignmentPoint(TooltipPosition.TopCenter).y - _canvasRectTrans.rect.height/2;
            _rectTrans.anchoredPosition = pos;
        }
        else if (GetAlignmentPoint(TooltipPosition.BottomCenter).y < -_canvasRectTrans.rect.height/2)
        {
            Debug.Log($"Tooltip going off on the bottom.");

            pos.y += -_canvasRectTrans.rect.height/2 - GetAlignmentPoint(TooltipPosition.BottomCenter).y;
            _rectTrans.anchoredPosition = pos;
        }
    }

    private Vector2 GetAlignmentOffset(TooltipPosition alignment)
    {
        // Get the distance between the pivot of the tooltip and the pivot of the selected alignment.
        Transform targetPivot = _tooltipPivots[(int)alignment];

        Vector3 worldOffset = targetPivot.position - _rectTrans.position;

        Vector3 localOffset = _rectTrans.parent.InverseTransformVector(worldOffset);

        return localOffset;
    }

    private Vector2 GetAlignmentPoint(TooltipPosition alignment)
    {
        Transform targetPivot = _tooltipPivots[(int)alignment];

        Vector3 worldPoint = targetPivot.position;

        Vector3 localPoint = _rectTrans.parent.InverseTransformPoint(worldPoint);

        return localPoint;
    }
    private Vector2 GetAlignmentPoint(RectTransform alignmentPivot)
    {
        Vector3 worldPoint = alignmentPivot.position;

        Vector3 localPoint = _rectTrans.parent.InverseTransformPoint(worldPoint);

        return localPoint;
    }

    // Animations 
    private void DoShowTooltipAnim()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1f, _showTooltipDuration);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            float headerLength    = _headerTMP.preferredWidth;
            float contentLength   = _contentTMP.preferredWidth;

            _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit);
        }
    }
}

public enum TooltipPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}