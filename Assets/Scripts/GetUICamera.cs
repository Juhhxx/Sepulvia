using UnityEngine;

public class GetUICamera : MonoBehaviour
{
    private Canvas _canvas;

    [SerializeField] private bool _onEnable = true;

    private void OnEnable()
    {
        if (_onEnable) GetCamera();
    }

    private void Start()
    {
        if (!_onEnable) GetCamera();
    }

    private void GetCamera()
    {
        if (_canvas == null) _canvas = GetComponent<Canvas>();

        _canvas.worldCamera = MenuManager.Instance.GetUICamera();
    }
}
