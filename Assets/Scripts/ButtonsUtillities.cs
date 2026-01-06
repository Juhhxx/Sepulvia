using NaughtyAttributes;
using UnityEngine;

public class ButtonsUtillities : MonoBehaviour
{
    [SerializeField, Scene] private string _sceneToLoad;

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
