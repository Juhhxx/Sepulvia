using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class DataBase<T> : ScriptableObject where T : DataAsset
{
    [SerializeField] protected List<T> _entries = new();
    public IReadOnlyList<T> Entries => _entries;

    protected Dictionary<int, T> _lookup;

    public bool IsEmpty => _entries.Count == 0;

    private void OnEnable()
    {
        BuildLookup();
    }

    protected void BuildLookup()
    {
        _lookup = new Dictionary<int, T>();

        foreach (var entry in _entries)
        {
            if (entry == null)
                continue;

            _lookup[entry.ID] = entry;
        }
    }

    public T Get(int id)
    {
        if (_lookup == null)
            BuildLookup();

        return _lookup.TryGetValue(id, out var result)
            ? result
            : null;
    }

#if UNITY_EDITOR
    [SerializeField] protected string _lookUpPath;

    [Button]
    public void RefreshDataBase()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { _lookUpPath });

        List<T> foundAssets = new List<T>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null) foundAssets.Add(asset);
        }

        AssignIDs(foundAssets);

        _entries = foundAssets;

        BuildLookup();

        EditorUtility.SetDirty(this);
    }

    private void AssignIDs(List<T> assets)
    {
        int nextID = GetNextAvailableID(assets);

        foreach (var asset in assets)
        {
            if (asset.ID > 0)
                continue;

            asset.SetID(nextID++);

            EditorUtility.SetDirty(asset);
        }
    }

    private int GetNextAvailableID(List<T> assets)
    {
        int highestID = -1;

        foreach (var asset in assets)
        {
            highestID = Mathf.Max(highestID, asset.ID);
        }

        return highestID + 1;
    }

#endif
}
