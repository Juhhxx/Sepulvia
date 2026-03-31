using UnityEngine;

public class GetActiveCamera : MonoBehaviour
{
    private Canvas _canvas;

    private void OnEnable()
    {
        if (_canvas == null) _canvas = GetComponent<Canvas>();

        _canvas.worldCamera = GameSceneManager.Instance.CurrentCamera;
    }
}
