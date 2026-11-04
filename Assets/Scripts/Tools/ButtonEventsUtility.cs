using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ButtonEventsUtility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnButtonDown;
    public event Action OnButtonUp;
    public event Action OnButtonEnter;
    public event Action OnButtonExit;
    public event Action<bool> OnButtonHover;
    public event Action<bool> OnButtonHold;

    private bool _isHovered;
    public bool IsHovered
    {
        get => _isHovered;

        private set
        {
            var tmp = value;

            OnButtonHover?.Invoke(value);

            _isHovered = value;
        }
    }

    private bool _isHeld;
    public bool IsHeld
    {
        get => _isHeld;
        
        private set
        {
            var tmp = value;

            OnButtonHold?.Invoke(value);

            _isHeld = value;
        }
    }

    private UnityEngine.UI.Button _button;

    private void Start()
    {
        _button = GetComponent<UnityEngine.UI.Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnButtonDown?.Invoke();

        IsHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnButtonUp?.Invoke();

        IsHeld = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnButtonEnter?.Invoke();

        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnButtonExit?.Invoke();

        IsHovered = false;
    }

    private void OnDisable()
    {
        if (IsHovered)
        {
            OnButtonExit?.Invoke();
        }

        if (IsHeld)
        {
            OnButtonUp?.Invoke();
        }

        IsHeld = false;
        IsHovered = false;
    }
}
