using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThresholdBarManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _barNameTMP;
    [SerializeField] private TextMeshProUGUI _barInfoTMP;
    [SerializeField] private Image _barFillImage;
    [SerializeField] private GameObject _barDivisorPrefab;
    [SerializeField] private Transform _barDivisorsParent;

    [SerializeField] private float _updateSpeed = 0.5f;

    private string _infoName;
    private float _maxValue;
    private GameObject[] _divisors; 

    public void SetUpBar(string name, string info, float maxValue)
    {
        _barNameTMP.text = name;
        //_barInfoTMP.text = $"{info} (0/{maxValue:f1})";
        _barFillImage.fillAmount = 0f; 

        _infoName = info;
        _maxValue = maxValue;

        CleanUpDivisors();
        
        int divisorCount = Mathf.FloorToInt(maxValue);
        _divisors = new GameObject[divisorCount];

        for (int i = 0; i < divisorCount; i++)
        {
            GameObject divisor = Instantiate(_barDivisorPrefab, _barDivisorsParent);
            _divisors[i] = divisor;
        }
    }

    private void CleanUpDivisors()
    {
        if (_divisors != null)
        {
            foreach (GameObject divisor in _divisors)
            {
                if (divisor != null)
                {
                    Destroy(divisor);
                }
            }
        }
    }

    private void UpdateDivisorColors(float currentAmount)
    {
        int reachedDivisors = Mathf.FloorToInt(currentAmount);

        Debug.Log($"CURRENT AMOUNT = {currentAmount} REACHED DIVISORS = {reachedDivisors}");

        for (int i = 0; i < _divisors.Length; i++)
        {
            Image divisorImage = _divisors[i].transform.GetChild(0).GetComponent<Image>();

            Debug.Log($"DIVISOR {i} SPRITE = {(i < reachedDivisors ? "REACHED" : "NORMAL")}");

            if (i < reachedDivisors)
            {
                divisorImage.gameObject.SetActive(true);
            }
            else
            {
                divisorImage.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateFillAmout(float newAmount)
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
            UpdateDivisorColors(newValue * _maxValue);

            yield return null;
        }

        UpdateUI(to);
        UpdateDivisorColors(to * _maxValue);
    }

    private void UpdateUI(float to)
    {
        string displayValue = $"{_maxValue * to:f1}";

        _barFillImage.fillAmount = to;
        _barInfoTMP.text = $"{_infoName} ({displayValue}/{_maxValue:f1})";
    }
}
