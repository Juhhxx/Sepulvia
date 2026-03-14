using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotManager : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountTMP;

    public void UpdateSlot(Sprite sprite, int amount)
    {
        _itemImage.sprite = sprite;
        _itemImage.color = Color.white;

        _itemAmountTMP.text = amount.ToString();
    }

    public void UpdateSlot(Sprite sprite)
    {
        _itemImage.sprite = sprite;
        _itemImage.color = Color.white;
    }

    public void UpdateSlot()
    {
        if (_itemImage != null) _itemImage.color = new Color(1f , 1f, 1f, 0f);
        if (_itemAmountTMP != null) _itemAmountTMP.text = "";
    }
}
