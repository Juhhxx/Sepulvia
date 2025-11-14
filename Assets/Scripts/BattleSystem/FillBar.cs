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

    [OnValueChanged("UpdateBar")]
    [SerializeField, Range(0, 1)] private float _barFillAmout;

    private string _infoName;
    private float _maxValue;

    public void SetUpBar(string name, string info, float maxValue)
    {
        _barNameTMP.text = name;
        _barInfoTMP.text = $"{info} ({maxValue}/{maxValue})";
        _barFillAmout = 1f;
        _barFillImage.fillAmount = 1f;

        _infoName = info;
        _maxValue = maxValue;
    }

    public void UpdateBar() => _barFillImage.fillAmount = _barFillAmout;

    public void UpdateFillAmout(int newAmount)
    {
        Debug.Log($"CURRENT HP : {newAmount}");
        StopAllCoroutines();
        StartCoroutine(UpdateBarCR(newAmount / _maxValue));
    }

    private IEnumerator UpdateBarCR(float to)
    {
        float from = _barFillAmout;
        float newValue = from;
        float i = 0;

        while (_barFillAmout != to)
        {
            newValue = Mathf.Lerp(from, to, i);

            _barFillImage.fillAmount = newValue;
            _barInfoTMP.text = $"{_infoName} ({(int)(_maxValue * newValue)}/{_maxValue})";
            _barFillAmout = newValue;

            i += Time.deltaTime * _updateSpeed;

            yield return null;
        }
    }
}
