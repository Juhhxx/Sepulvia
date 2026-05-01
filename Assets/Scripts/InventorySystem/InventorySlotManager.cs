using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventorySlotManager : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountTMP;
    [SerializeField] private GameObject _selectionImage;

    private void OnDisable()
    {
        _selectionImage.SetActive(false);
    }

    public void UpdateSlot(Sprite sprite, int amount)
    {
        _itemImage.sprite = sprite;
        _itemImage.color = Color.white;

        _itemAmountTMP.text = amount.ToString();
        _itemAmountTMP.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateSlot(Sprite sprite)
    {
        _itemImage.sprite = sprite;
        _itemImage.color = Color.white;
    }

    public void UpdateSlot()
    {
        if (_itemImage != null) _itemImage.color = new Color(1f , 1f, 1f, 0f);
        if (_itemAmountTMP != null)
        {
            _itemAmountTMP.text = "";
            _itemAmountTMP.transform.parent.gameObject.SetActive(false);
        }
    }

    // Animations

    private float _scaleAmount = 1.15f;
    private float _animDuration = 0.5f;

    public void DoSlotSelectAnim()
    {
        if (_itemImage == null) return;
        _itemImage.transform.DOScale(Vector3.one * _scaleAmount, _animDuration).SetEase(Ease.OutElastic);
    }

    public void DoSlotDeselectAnim()
    {
        if (_itemImage == null) return;
        _itemImage.transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.OutElastic);
    }
}
