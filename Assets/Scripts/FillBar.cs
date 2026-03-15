using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private bool _3D = false;

    [SerializeField] private TextMeshProUGUI _barNameTMP;
    [SerializeField] private TextMeshProUGUI _barInfoTMP;
    [SerializeField] private Image _barFillImage;

    [SerializeField, ShowIf("_3D")] private Canvas _canvas;
    [SerializeField] private float _updateSpeed = 0.5f;

    private string _infoName;
    private float _maxValue;

    public void SetUpBar(string name, string info, float maxValue)
    {
        if (_3D)
        {
            _canvas.worldCamera = GameSceneManager.Instance.CurrentCamera;
        }

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
        if (gameObject.activeInHierarchy) StartCoroutine(UpdateBarCR(newAmount / _maxValue));
    }

    private IEnumerator UpdateBarCR(float to)
    {
        float from = _barFillImage.fillAmount;

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

        _barFillImage.fillAmount = to;
        _barInfoTMP.text = $"{_infoName} ({displayValue}/{_maxValue})";
    }
}
