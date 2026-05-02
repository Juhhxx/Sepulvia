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

        if (_itemAmountTMP.text != "")
        {
            int previousAmount = int.Parse(_itemAmountTMP.text);
            if (previousAmount != amount) DoAmountChangeAnim();
        }
        
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

    public void SetDisabled()
    {
        if (_itemImage != null) _itemImage.color = new Color(1f, 1f, 1f, 0.35f);
    }

    // Animations

    private float _scaleAmount = 1.15f;
    private float _scaleAmountNumber = 1.25f;
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

    public void DoAmountChangeAnim()
    {
        if (_itemAmountTMP == null) return;

        _itemAmountTMP.transform.DOScale(Vector3.one * _scaleAmountNumber, 0.1f).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            _itemAmountTMP.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
        });
    }
}
