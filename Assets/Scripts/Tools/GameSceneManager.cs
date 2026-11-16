using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameSceneManager : MonoBehaviourSingleton<GameSceneManager>
{
    public enum GameSceneTypes { Overworld, Battle }

    [Serializable]
    public struct GameScene
    {
        // General
        [field: Header("General")]
        [field: Space(5f)]
        [field: SerializeField] public GameSceneTypes SceneType { get; private set; }
        [field: SerializeField] public Camera SceneCamera { get; private set; }
        [field: SerializeField] public PlaySound SceneBackground { get; private set; }

        // Wipe Transition
        [field: SerializeField] public ScreenWipeTrigger WipeTransition { get; private set; }

        // Fog
        [field: Space(10f)]
        [field: Header("Objects")]
        [field: Space(5f)]
        [field: SerializeField] public GameObject SceneGameObject { get; private set; }

        // Fog
        [field: Space(10f)]
        [field: Header("Fog")]
        [field: Space(5f)]
        [field: SerializeField] public bool Fog { get; private set; }
        
        [field: AllowNesting]
        [field: SerializeField, ShowIf("Fog")] public Color FogColor { get; private set; }

        [field: AllowNesting]
        [field: SerializeField, ShowIf("Fog")] public FogMode FogMode { get; private set; }

        [field: AllowNesting]
        [field: SerializeField, Range(0,1), ShowIf("Fog")] public float FogDensity { get; private set; }

        [field: AllowNesting]
        [field: SerializeField, Range(0,1), ShowIf("Fog")] public float HaloStrenght { get; private set; }
    }

    [SerializeField] private GameSceneTypes _currentGameScene;
    public GameSceneTypes CurrentGameScene
    {
        get => _currentGameScene;

        set
        {
            if (_currentGameScene != value)
            {
                UpdateGameScene(GetGameSceneByType(value));
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    public void ChangeScene()
    {
        CurrentGameScene = _currentGameScene == GameSceneTypes.Overworld ? GameSceneTypes.Battle : GameSceneTypes.Overworld; 
    }

    public void ChangeScene(GameSceneTypes gameScene, Action onChangeDone = null)
    {
        UpdateGameScene(GetGameSceneByType(gameScene), onChangeDone);
    }

    [SerializeField, ReorderableList] private List<GameScene> _gameScenes;

    public Camera CurrentCamera { get; private set; }

    public Action OnGameSceneChange;

    private GameScene GetGameSceneByType(GameSceneTypes type)
    {
        foreach(GameScene gs in _gameScenes)
        {
            if (gs.SceneType == type) return gs;
        }

        Debug.Log($"[Game Scene Manager] Game Scene of Type {type} not found!", this);
        return default;
    }

    private void UpdateGameScene(GameScene scene, Action onChangeDone = null)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateGameSceneCR(scene, onChangeDone));
    }

    private void UpdateGameSceneImmediate(GameScene scene)
    {
        Debug.Log($"[Game Scene Manager] Changing from Game Scene {_currentGameScene} to {scene.SceneType}", this);

        // Disable Previous Camera and Scene Objects
        GetGameSceneByType(_currentGameScene).SceneCamera.enabled = false;
        GetGameSceneByType(_currentGameScene).SceneGameObject.SetActive(false);

        _currentGameScene = scene.SceneType;

        SetGameScene(scene);
    }

    private IEnumerator UpdateGameSceneCR(GameScene scene, Action onChangeDone = null)
    {
        Debug.Log($"[Game Scene Manager] Changing from Game Scene {_currentGameScene} to {scene.SceneType}", this);

        scene.WipeTransition?.TriggerWipe(true);
        yield return new WaitUntil(() => !(scene.WipeTransition?.CurrentWipe?.IsPlaying) ?? true);

        // Disable Previous Camera and Scene Objects
        GetGameSceneByType(_currentGameScene).SceneCamera.enabled = false;
        GetGameSceneByType(_currentGameScene).SceneGameObject.SetActive(false);

        _currentGameScene = scene.SceneType;

        SetGameScene(scene);

        OnGameSceneChange?.Invoke();
        onChangeDone?.Invoke();

        scene.WipeTransition?.TriggerWipe(false);
        yield return new WaitUntil(() => !(scene.WipeTransition?.CurrentWipe?.IsPlaying) ?? true);
    }

    private void SetGameScene(GameScene scene)
    {
        scene.SceneCamera.enabled = true;

        CurrentCamera = scene.SceneCamera;

        if (Application.isPlaying) scene.SceneBackground?.SoundPlay();

        scene.SceneGameObject.SetActive(true);

        RenderSettings.fog = scene.Fog;

        if (scene.Fog)
        {
            RenderSettings.fogColor = scene.FogColor;
            RenderSettings.fogMode = scene.FogMode;
            RenderSettings.fogDensity = scene.FogDensity;
            RenderSettings.haloStrength = scene.HaloStrenght;
        }
    }

    private void ActivateSceneNoCamera(GameScene scene)
    {
        scene.SceneGameObject.SetActive(true);
    }

    private void DeActivateSceneNoCamera(GameScene scene)
    {
        scene.SceneGameObject.SetActive(false);
    }

    private void DisableOtherScenes()
    {
        foreach(GameScene gs in _gameScenes)
        {
            if (gs.SceneType != _currentGameScene)
            {
                gs.SceneCamera.enabled = false;
                gs.SceneGameObject.SetActive(false);
            }
        }
    }
    
    private void Awake()
    {
        base.SingletonCheck(this, false);

        UpdateGameSceneImmediate(GetGameSceneByType(_currentGameScene));
        DisableOtherScenes();

        // StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        foreach(GameScene gs in _gameScenes)
        {
            ActivateSceneNoCamera(gs);
            yield return new WaitForEndOfFrame();

            if (gs.SceneType != CurrentGameScene) DeActivateSceneNoCamera(gs);
        }
    }
}