using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NaughtyAttributes;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string _saveFileName;
    [SerializeField] private bool _prettyPrint;
    private string _saveFilePath;

    // Save Sytem Events
    public event Action OnGameSaveStart;
    public event Action OnGameSaveEnd;
    public event Action OnGameLoadStart;
    public event Action OnGameLoadEnd;

    // Saving Information
    private Dictionary<string, ISaveable> _saveData;

    // Singleton Pattern Instance
    public static SaveManager Instance; 

    private void SingletonCheck()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Awake()
    {
        SingletonCheck();

        _saveFilePath = System.IO.Path.Combine(Application.persistentDataPath, _saveFileName) + ".savedata";
        _saveData = new Dictionary<string, ISaveable>();
    }

    // Register Saveable Objects
    public void RegsiterSaveable(string name, ISaveable saveable)
    {
        Debug.Log($"REGISTERED {name}");

        _saveData.Add(name, saveable);
    }

    // Save and Load Game
    [Button]
    public async void SaveGame()
    {
        OnGameSaveStart?.Invoke();

        GameSave gameSave;
        gameSave.saveData = new List<SaveData>();

        List<SaveData> saveDataList = new List<SaveData>();

        foreach (string name in _saveData.Keys)
        {
            SaveData saveData;
            saveData.name = name;
            saveData.data = _saveData[name].GetSaveData();

            Debug.Log($"SAVING {saveData.name}");

            saveDataList.Add(saveData);
        }

        gameSave.saveData = saveDataList;

        string jsonSaveData = JsonConvert.SerializeObject(gameSave, Formatting.Indented);

        // Wait until the file is written without freezing the game
        await File.WriteAllTextAsync(_saveFilePath, jsonSaveData);

        Debug.Log($"GAME SAVED TI {_saveFilePath}");
        OnGameSaveEnd?.Invoke();
    }
    
    [Button]
    public async Task LoadGame()
    {
        if (File.Exists(_saveFilePath))
        {
            OnGameLoadStart?.Invoke();
            // Wait until the file is read without freezing the game
            string jsonSaveData = await File.ReadAllTextAsync(_saveFilePath);

            GameSave gameSave = JsonConvert.DeserializeObject<GameSave>(jsonSaveData);

            foreach (SaveData save in gameSave.saveData)
            {
                _saveData[save.name].LoadSaveData(save.data);
            }

            OnGameLoadEnd?.Invoke();
        }
        else
        {
            Debug.Log("NO SAVE FILE FOUND");
        }
    }

    // Data Structures
    private struct SaveData
    {
        public string name;
        [SerializeReference]
        public object data;
    }
    [Serializable]
    private struct GameSave
    {
        [SerializeReference]
        public List<SaveData> saveData;
    }
}
