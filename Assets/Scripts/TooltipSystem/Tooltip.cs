using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [Header("Tooltip Layout Settings")]
    [SerializeField] private TextMeshProUGUI _headerTMP;
    [SerializeField] private TextMeshProUGUI _contentTMP;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private float _characterWrapLimit;

    [Header("Tooltip Pivot Settings")]
    [SerializeField] private List<RectTransform> _tooltipPivots = new List<RectTransform>();

    [Header("Tooltip Additional Info Settings")]

    [SerializeField] private Transform _additionalParent;
    [SerializeField] private GameObject _additionalPrefab;

    private RectTransform _rectTrans;
    public RectTransform RectTransform => _rectTrans;

    [SerializeField] private List<GameObject> _additionalTooltips = new List<GameObject>();

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
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

    // Position Tooltip
    public void PositionTooltip(Vector2 position, TooltipPosition alignment)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTrans);

        var offset = GetAlignmentOffset(alignment);

        Vector2 pos = position - offset;

        _rectTrans.anchoredPosition = pos;
    }

    private Vector2 GetAlignmentOffset(TooltipPosition alignment)
    {
        // Get the distance between the pivot of the tooltip and the pivot of the selected alignment.
        Transform targetPivot = _tooltipPivots[(int)alignment];

        Vector3 worldOffset = targetPivot.position - _rectTrans.position;

        Vector3 localOffset = _rectTrans.parent.InverseTransformVector(worldOffset);

        return localOffset;
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