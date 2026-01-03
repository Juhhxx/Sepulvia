using System;
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
    private void ChangeScene()
    {
        CurrentGameScene = _currentGameScene == GameSceneTypes.Overworld ? GameSceneTypes.Battle : GameSceneTypes.Overworld; 
    }

    [SerializeField, ReorderableList] private List<GameScene> _gameScenes;

    public Camera CurrentCamera { get; private set; }

    private GameScene GetGameSceneByType(GameSceneTypes type)
    {
        foreach(GameScene gs in _gameScenes)
        {
            if (gs.SceneType == type) return gs;
        }

        Debug.Log($"[Game Scene Manager] Game Scene of Type {type} not found!", this);
        return default;
    }

    private void UpdateGameScene(GameScene scene)
    {
        Debug.Log($"[Game Scene Manager] Changing from Game Scene {_currentGameScene} to {scene.SceneType}", this);

        // Disable Previous Camera and Scene Objects
        GetGameSceneByType(_currentGameScene).SceneCamera.enabled = false;
        GetGameSceneByType(_currentGameScene).SceneGameObject.SetActive(false);

        _currentGameScene = scene.SceneType;

        SetGameScene(scene);
    }

    private void SetGameScene(GameScene scene)
    {
        scene.SceneCamera.enabled = true;

        CurrentCamera = scene.SceneCamera;

        scene.SceneGameObject.SetActive(true);
        Debug.LogWarning($"{scene.SceneGameObject.activeInHierarchy}");

        RenderSettings.fog = scene.Fog;

        if (scene.Fog)
        {
            RenderSettings.fogColor = scene.FogColor;
            RenderSettings.fogMode = scene.FogMode;
            RenderSettings.fogDensity = scene.FogDensity;
            RenderSettings.haloStrength = scene.HaloStrenght;
        }
    }
    
    private void Awake()
    {
        base.SingletonCheck(this, false);

        UpdateGameScene(GetGameSceneByType(_currentGameScene));
    }
}