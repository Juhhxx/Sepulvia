using UnityEngine;

public class GetUICamera : MonoBehaviour
{
    private Canvas _canvas;

    private void OnEnable()
    {
        if (_canvas == null) _canvas = GetComponent<Canvas>();

        _canvas.worldCamera = MenuManager.Instance.GetUICamera();
    }
}
