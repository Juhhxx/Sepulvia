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

    private List<GameObject> _additionalTooltips = new List<GameObject>();

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

    public void SetText(string context, string header = "", List<TooltipInfo> addInfos = null)
    {
        if (string.IsNullOrEmpty(header))
        {
            _headerTMP.gameObject.SetActive(false);
        }
        else
        {
            _headerTMP.gameObject.SetActive(true);
        }

        _headerTMP.text = header;
        _contentTMP.text = context;

        int headerLength    = _headerTMP.text.Length;
        int contentLength   = _contentTMP.text.Length;

        _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit);

        ShowAdditionalTooltips(addInfos);
    }

    private void ShowAdditionalTooltips(List<TooltipInfo> infos)
    {
        ClearAdditionalTooltips();

        if (infos == null) return;

        foreach (TooltipInfo info in infos)
        {
            var tt = Instantiate(_additionalPrefab, _additionalParent);

            tt.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            _additionalTooltips.Add(tt);
            
            TextMeshProUGUI tmp = tt.GetComponentInChildren<TextMeshProUGUI>();

            tmp.text = info.Content;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_additionalParent.GetComponent<RectTransform>());
    }

    private void ClearAdditionalTooltips()
    {
        foreach (GameObject tt in _additionalTooltips)
        {
            Destroy(tt);
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
