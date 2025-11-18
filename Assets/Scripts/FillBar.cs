using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private bool _3D = false;

    [SerializeField, HideIf("_3D")] private TextMeshProUGUI _barNameTMP;
    [SerializeField, HideIf("_3D")] private TextMeshProUGUI _barInfoTMP;
    [SerializeField, HideIf("_3D")] private Image _barFillImage;

    [SerializeField, ShowIf("_3D")] private TextMeshPro _barNameTMP3D;
    [SerializeField, ShowIf("_3D")] private TextMeshPro _barInfoTMP3D;
    [SerializeField, ShowIf("_3D")] private Transform _barFillImage3D;

    [SerializeField] private float _updateSpeed = 0.5f;

    private string _infoName;
    private float _maxValue;

    public void SetUpBar(string name, string info, float maxValue)
    {
        if (_3D)
        {
            _barNameTMP3D.text = name;
            _barInfoTMP3D.text = $"{info} ({maxValue}/{maxValue})";

            Vector3 newScale = _barFillImage3D.localScale;
            newScale.x = 1f;
            _barFillImage3D.localScale = newScale;
        }
        else
        {
           _barNameTMP.text = name;
            _barInfoTMP.text = $"{info} ({maxValue}/{maxValue})";
            _barFillImage.fillAmount = 1f; 
        }

        _infoName = info;
        _maxValue = maxValue;
    }

    public void UpdateFillAmout(int newAmount)
    {
        Debug.Log($"CURRENT HP : {newAmount}");
        StopAllCoroutines();
        StartCoroutine(UpdateBarCR(newAmount / _maxValue));
    }

    private IEnumerator UpdateBarCR(float to)
    {
        float from = _3D ? _barFillImage3D.localScale.x : _barFillImage.fillAmount;

        if (to == from) StopAllCoroutines();

        float newValue = from;
        float i = 0;

        while (i <= 1)
        {
            i += Time.deltaTime * _updateSpeed;

            float t = Mathf.Clamp01(i);

            newValue = Mathf.Lerp(from, to, t);

            UpdateUI(newValue);

            yield return null;
        }

        UpdateUI(to);        
    }

    private void UpdateUI(float to)
    {
        int displayValue = Mathf.RoundToInt(_maxValue * to);

        if (_3D)
        {
            Vector3 newScale = _barFillImage3D.localScale;
            newScale.x = to;
            _barFillImage3D.localScale = newScale;
            _barInfoTMP3D.text = $"{_infoName} ({displayValue}/{_maxValue})";
        }
        else
        {
            _barFillImage.fillAmount = to;
            _barInfoTMP.text = $"{_infoName} ({displayValue}/{_maxValue})";
        }
    }
}
