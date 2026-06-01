using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopDisplayManager : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _priceTag;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    [SerializeField] private Image _selectionImage;

    private void Start()
    {
        _selectionImage.transform.localScale = Vector3.zero;
    }
    
    public void UpdateDisplay(Sprite sprite, int price)
    {
        _image.sprite = sprite;

        _priceTMP.text = price.ToString();
        _priceTag.SetActive(true);

        DoUpdateDisplayAnim();
    }

    // Animations

    private float _scaleAmount = 1.15f;
    private float _animDuration = 0.15f;

    public void DoUpdateDisplayAnim()
    {
        transform.DOPunchScale(Vector3.one * _scaleAmount, _animDuration).SetEase(Ease.Linear);
    }

    public void DoDisplaySelectAnim()
    {
        transform.DOScale(Vector3.one * _scaleAmount, _animDuration).SetEase(Ease.Linear);
        _selectionImage.transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.Linear);
    }
    public void DoDisplayDeselectAnim()
    {
        transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.Linear);
        _selectionImage.transform.DOScale(Vector3.zero, _animDuration).SetEase(Ease.Linear);
    }

    public void DoDisplayPurchaseAnim()
    {
        _image.transform.DOShakeRotation(_animDuration, new Vector3(0, 0, 15), 10, 90).SetEase(Ease.Linear);
    }

    public void DoDisplayNotEnoughAnim()
    {
        _priceTag.transform.DOShakePosition(_animDuration, new Vector3(10, 0, 0), 10, 90).SetEase(Ease.Linear);
    }

}
