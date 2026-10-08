using UnityEngine;

public class GetActiveCameraOnBattleStart : MonoBehaviour
{
    private Canvas _canvas;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _playerController.OnBattleEnterExit += (enteredBattle) => {
            if (enteredBattle) AssignCamera();
        };
    }

    private void AssignCamera()
    {
        if (_canvas == null) _canvas = GetComponent<Canvas>();

        _canvas.worldCamera = GameSceneManager.Instance?.CurrentCamera;
    }
}
