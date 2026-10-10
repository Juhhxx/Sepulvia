using UnityEngine;
using UnityEngine.SceneManagement;

public class GetActiveCameraOnBattleStart : MonoBehaviour
{
    private Canvas _canvas;
    private PlayerController _playerController;

    private void Start()
    {
        SceneManager.sceneLoaded += (_,_) => FindPlayer();
    }

    private void FindPlayer()
    {
        _playerController = FindAnyObjectByType<PlayerController>();

        if (_playerController == null) return;

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
