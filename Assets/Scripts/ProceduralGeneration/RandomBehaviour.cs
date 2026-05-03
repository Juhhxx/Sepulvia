using System;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class RandomBehaviour : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _uniqueID;
    public string UniqueID => _uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (string.IsNullOrWhiteSpace(_uniqueID) || IsDuplicate(_uniqueID))
        {
            _uniqueID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }

    private bool IsDuplicate(string id)
    {
        var all = FindObjectsByType<RandomBehaviour>(0);
        return all.Count(r => r != this && r.UniqueID == id) > 0;
    }
#endif

    [SerializeField, ReadOnly] private int _seed;

    protected System.Random _random;

    protected void TryInitializeRandom()
    {
        var parentRandom = transform.parent?.GetComponentInParent<RandomBehaviour>();

        if (parentRandom != null)
        {
            Debug.Log($"[Random Behaviour : {name}] Random has parents", this);
            return;
        }

        _seed = SeedManager.Instance.GetSeed(gameObject.name, _uniqueID);

        Initialize(_seed);

        var childRandoms = GetComponentsInChildren<RandomBehaviour>().ToList();

        if (childRandoms.Count > 1)
        {
            Debug.Log($"[Random Behaviour : {name}] Random has children, initializing them", this);
            InitializeChildren(_seed);
        }
    }
    public void Initialize(int seed)
    {
        gameObject.name += $" - Seed : {seed}";

        _seed = seed;
        _random = new System.Random(seed);
    }
    public void InitializeChildren(int seed)
    {
        var childRandoms = GetComponentsInChildren<RandomBehaviour>().ToList();

        foreach (RandomBehaviour rnd in childRandoms)
        {
            if (rnd == this) continue;

            int childSeed = SeedManager.Instance.GetSeed(rnd.gameObject.name, seed, rnd.UniqueID);
            rnd.Initialize(childSeed);
        }
    }
}