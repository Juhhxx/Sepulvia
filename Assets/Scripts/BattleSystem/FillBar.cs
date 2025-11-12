using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _barNameTMP;
    [SerializeField] private TextMeshProUGUI _barInfoTMP;
    [SerializeField] private Image _barFillImage;

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
        _barInfoTMP.text = $"{_infoName} ({newAmount}/{_maxValue})";
        _barFillAmout = _maxValue / newAmount;
    }
}
