using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _barNameTMP;
    [SerializeField] private TextMeshProUGUI _barInfoTMP;
    [SerializeField] private Image _barFillImage;
    [SerializeField] private float _updateSpeed = 0.5f;

    private string _infoName;
    private float _maxValue;

    public void SetUpBar(string name, string info, float maxValue)
    {
        _barNameTMP.text = name;
        _barInfoTMP.text = $"{info} ({maxValue}/{maxValue})";
        _barFillImage.fillAmount = 1f;

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
        float from = _barFillImage.fillAmount;
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
        _barFillImage.fillAmount = to;

        int displayValue = Mathf.RoundToInt(_maxValue * to);
        
        _barInfoTMP.text = $"{_infoName} ({displayValue}/{_maxValue})";
    }
}
