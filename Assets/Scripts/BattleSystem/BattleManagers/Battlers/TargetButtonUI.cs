using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _characterNameTMP;
    [SerializeField] private Image _characterIconImage;
    [SerializeField] private Image _selectionBorder;

    private bool _isSelected = false;

    public void SetUp(Character c)
    {
        _characterNameTMP.text = c.Name;

        ToggleSelect(false);
    }

    public void ToggleSelect()
    {
        _isSelected = !_isSelected;

        _selectionBorder.enabled = _isSelected;
    }
    private void ToggleSelect(bool onOff)
    {
        _isSelected = onOff;

        _selectionBorder.enabled = _isSelected;
    }
}
