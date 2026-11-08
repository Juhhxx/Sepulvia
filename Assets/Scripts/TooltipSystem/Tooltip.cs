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
    [SerializeField] private int _characterWrapLimit;

    [SerializeField] private Transform _additionalParent;
    [SerializeField] private GameObject _additionalPrefab;

    private RectTransform _rectTrans;
    public RectTransform RectTransform => _rectTrans;

    [SerializeField] private List<GameObject> _additionalTooltips = new List<GameObject>();

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

    public void ToggleTooltip(bool show, TooltipInfo info = null, List<TooltipInfo> addInfos = null)
    {
        if (!show)
        {
            ClearAdditionalTooltips();
            gameObject.SetActive(show);
        }
        else
        {
            gameObject.SetActive(show);
            SetText(info, addInfos);
        }

    }

    private void SetText(TooltipInfo info, List<TooltipInfo> addInfos = null)
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

        int headerLength    = _headerTMP.text.Length;
        int contentLength   = _contentTMP.text.Length;

        _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit);

        ShowAdditionalTooltips(addInfos);
    }

    private void ShowAdditionalTooltips(List<TooltipInfo> infos)
    {
        if (infos == null) return;

        foreach (TooltipInfo info in infos)
        {
            var tt = Instantiate(_additionalPrefab, _additionalParent);
            Debug.Log($"Instantiated additional tooltip: {tt.name}");

            _additionalTooltips.Add(tt);
            
            TextMeshProUGUI tmp = tt.GetComponentInChildren<TextMeshProUGUI>();

            tmp.text = info.Content;
        }

        Canvas.ForceUpdateCanvases();

        // Rebuild the vertical layout first.
        RectTransform additionalRect = _additionalParent.GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(additionalRect);

        // Then rebuild the horizontal layout containing the main tooltip.
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            _rectTrans
        );

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
            int headerLength    = _headerTMP.text.Length;
            int contentLength   = _contentTMP.text.Length;

            _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit);
        }
    }
}
