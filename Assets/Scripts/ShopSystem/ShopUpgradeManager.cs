using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopUpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statNameTMP;
    [SerializeField] private TextMeshProUGUI _statLevelTMP;
    [SerializeField] private TextMeshProUGUI _statValueTMP;
    [SerializeField] private TextMeshProUGUI _upgradeCostTMP;
    [SerializeField] private Image _statIconImage;

    public void SetUpDisplay(string statName, int statLevel, int statValue, Sprite statIcon)
    {
        _statNameTMP.text = statName;
        _statLevelTMP.text = $"Level {statLevel}";
        _statValueTMP.text = $"{statValue}";
        _statIconImage.sprite = statIcon;

        _upgradeCostTMP.gameObject.SetActive(false);
    }

    public void UpdateDisplay(int statLevel, int statValue)
    {
        _statLevelTMP.text = $"Level {statLevel}";
        _statValueTMP.text = $"{statValue}";

        _upgradeCostTMP.gameObject.SetActive(false);
    }

    public void ShowUpgrade(int upgradeCost, int amountGained, int statLevel, int statValue)
    {
        _upgradeCostTMP.text = $"{upgradeCost}";
        _upgradeCostTMP.gameObject.SetActive(true);

        _statLevelTMP.text = $"Level {statLevel} -> <color=green>Level {statLevel + 1}</color>";
        _statValueTMP.text = $"{statValue} -> <color=green>{statValue + amountGained}</color>";
    }

    public void ResetSelection() => MenuManager.Instance.ResetSelection();

    // Animations

    private float _scaleAmount = 1.05f;
    private float _animDuration = 0.15f;

    public void DoDisplaySelectAnim()
    {
        transform.DOScale(Vector3.one * _scaleAmount, _animDuration).SetEase(Ease.Linear);
    }
    public void DoDisplayDeselectAnim()
    {
        transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.Linear);
    }

    public void DoDisplayPurchaseAnim()
    {
        transform.DOShakeRotation(_animDuration, new Vector3(0, 0, 15), 10, 90).SetEase(Ease.Linear);
    }

    public void DoDisplayNotEnoughAnim()
    {
        transform.DOShakePosition(_animDuration, new Vector3(10, 0, 0), 10, 90).SetEase(Ease.Linear);
    }
}
