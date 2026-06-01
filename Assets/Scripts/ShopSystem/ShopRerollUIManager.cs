using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopRerollUIManager : MonoBehaviour
{
    [SerializeField] private Image _highlight;
    [SerializeField] private TextMeshProUGUI _rerollCostTmp;
    [SerializeField] private Transform _priceTag;

    private void Start()
    {
        _highlight.transform.localScale = Vector3.zero;
        _priceTag.gameObject.SetActive(false);
    }
    public void UpdateDisplay(int rerollCost)
    {
        _rerollCostTmp.text = rerollCost.ToString();
        _priceTag.gameObject.SetActive(false);
    }

    // Animations
    private float _scaleAmount = 1.15f;
    private float _animDuration = 0.15f;

    public void DoSelectAnim()
    {
        transform.DOScale(Vector3.one * _scaleAmount, _animDuration).SetEase(Ease.Linear);
        _highlight.transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.Linear);
        _priceTag.gameObject.SetActive(true);

    }
    public void DoDeselectAnim()
    {
        transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.Linear);
        _highlight.transform.DOScale(Vector3.zero, _animDuration).SetEase(Ease.Linear);
        _priceTag.gameObject.SetActive(false);
    }

    public void DoPurchaseAnim()
    {
        transform.DOShakeRotation(_animDuration, new Vector3(0, 0, 15), 10, 90).SetEase(Ease.Linear);
    }

    public void DoNotEnoughAnim()
    {
        _priceTag.transform.DOShakeScale(_animDuration, 0.5f).SetEase(Ease.Linear);
    }
}
