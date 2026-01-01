using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourSingleton<GameSceneManager>
{
    [SerializeField, ReadOnly] private List<Scene> _loadedScenes = new List<Scene>();

    public Scene CurrentActiveScene => _loadedScenes[0];

    public event Action<Scene> OnSceneChanged;

    private void Awake()
    {
        base.SingletonCheck(this, true);

        _loadedScenes.Add(SceneManager.GetActiveScene());
    }

    public async Task LoadNewSceneAsync(string sceneName, bool activate = false, Action onSceneActive = null)
    {
        Debug.Log($"[Game Scene Manager] Loading Scene {sceneName}");

        Scene scene = SceneManager.GetSceneByName(sceneName);

        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        _loadedScenes.Add(scene);

        Debug.Log($"[Game Scene Manager] Loaded Scene {sceneName}");

        if (activate) ActivateScene(sceneName, onSceneActive);
    }

    public async Task UnloadNewSceneAsync(string sceneName)
    {
        Debug.Log($"[Game Scene Manager] Unloading Scene {sceneName}");

        Scene scene = SceneManager.GetSceneByName(sceneName);

        await SceneManager.UnloadSceneAsync(sceneName);

        _loadedScenes.Remove(scene);

        Debug.Log($"[Game Scene Manager] Unloaded Scene {sceneName}");
    }

    public void ActivateScene(string sceneName, Action onSceneActive = null)
    {
        Debug.Log($"[Game Scene Manager] Setting Scene {sceneName} as Active Scene");

        Scene scene = SceneManager.GetSceneByName(sceneName);

        _loadedScenes.Remove(scene);
        _loadedScenes.Insert(0, scene);

        SceneManager.SetActiveScene(scene);

        OnSceneChanged?.Invoke(scene);
        onSceneActive?.Invoke();

        Debug.Log($"[Game Scene Manager] Set Scene {sceneName} as Active Scene");


    }
}
