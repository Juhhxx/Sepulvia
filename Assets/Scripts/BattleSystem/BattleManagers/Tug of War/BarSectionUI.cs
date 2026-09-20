using System;
using UnityEngine;
using UnityEngine.UI;

public class BarSectionUI : MonoBehaviour
{
    [SerializeField] private Image _sectionDivisorRight;
    [SerializeField] private Image _sectionDivisorLeft;
    [SerializeField] private Image[] _sectionChains;

    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _hasHeartColor;
    [SerializeField] private Color _isSelectedColor;
    private Color _currentColor;

    private BarSection _barSection;
    private PointerButtonEvents _pointerEvents;

    private void Awake()
    {
        _currentColor = _normalColor;

        _barSection = GetComponent<BarSection>();

        _barSection.OnHeartChange += SetHeartColor;
        _barSection.OnChangeConnections += () => 
            ToggleSectionDivisors(_barSection.ConnectRight != null, 
                _barSection.ConnectLeft != null);

        _pointerEvents = GetComponent<PointerButtonEvents>();

        _pointerEvents.OnPointerEnterEvent.AddListener(() => ToggleSelection(true));
        _pointerEvents.OnPointerExitEvent.AddListener(() => ToggleSelection(false));
    }

    private void SetHeartColor(bool hasHeart)
    {
        _currentColor = hasHeart ? _hasHeartColor : _normalColor;
        ChangeImagesColor(_currentColor);
    }
    private void ToggleSelection(bool isSelected)
    {
        if (isSelected && !_barSection.Button.enabled) return;

        var color = isSelected ? _isSelectedColor : _currentColor;

        ChangeImagesColor(color);
    }

    private void ChangeImagesColor(Color color)
    {
        foreach (Image i in _sectionChains)
        {
            i.color = color;
        }
    }

    private void ToggleSectionDivisors(bool right, bool left)
    {
        _sectionDivisorRight.enabled = right;
        _sectionDivisorLeft.enabled = left;
    }


}