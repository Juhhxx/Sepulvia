using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerTMP;
    [SerializeField] private TextMeshProUGUI _contentTMP;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private float _characterWrapLimit;

    [SerializeField] private Transform _additionalParent;
    [SerializeField] private GameObject _additionalPrefab;

    private RectTransform _rectTrans;
    public RectTransform RectTransform => _rectTrans;

    [SerializeField] private List<GameObject> _additionalTooltips = new List<GameObject>();

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

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
