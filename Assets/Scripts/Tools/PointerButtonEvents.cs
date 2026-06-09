using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public UnityEvent OnPointerEnterEvent;
    public UnityEvent OnPointerExitEvent;
    public UnityEvent OnPointerDownEvent;

    public bool IsPointerOver { get; private set; }
    private UnityEngine.UI.Button _button;

    private void Start()
    {
        _button = GetComponent<UnityEngine.UI.Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;

        OnPointerDownEvent?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnPointerEnterEvent?.Invoke();
        IsPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_button != null && (!_button?.interactable).Value) return;
        OnPointerExitEvent?.Invoke();
        IsPointerOver = false;
    }
}
