using UnityEngine;
using System;
using System.Text;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;

public class SeedManager : MonoBehaviourSingleton<SeedManager>
{
    [SerializeField] private int _seedMaxSize = 999999;
    private int _digits = 0;

    [SerializeField] private int _seed;
    public int Seed => _seed;

    private void Awake()
    {
        base.SingletonCheck(this);

        _digits = _seedMaxSize.ToString().Length;
    }

    public string SetSeed(string seedInput)
    {
        if (string.IsNullOrEmpty(seedInput))
        {
            GenerateSeed();
            return "";
        }
        
        if (int.TryParse(seedInput, out int newSeed))
        {
            if (newSeed < -_seedMaxSize || newSeed > _seedMaxSize)
            {
                return $"Seed must be between {-_seedMaxSize} and {_seedMaxSize}";
            }
            _seed = newSeed;
        }
        else
        {
            return "Invalid seed input. Please enter a valid integer.";
        }

        return "";
    }

    private void GenerateSeed()
    {
        int min = (int)Math.Pow(10, _digits - 1);

        _seed = UnityEngine.Random.Range(min, _seedMaxSize);
    }

    public int GetSeed(string name, string key)
    {
        int seed = Hash(_seed, key);

        Debug.Log($"[Seed Manager] {name} CREATED SEED : {seed}", this);

        return seed;
    }
    
    public int GetSeed(string name, int providedSeed, string key)
    {
        int seed = Hash(providedSeed, key);

        Debug.Log($"[Seed Manager] {name} CREATED SEED : {seed}", this);

        return seed;
    }

    private const ulong FNV_OFFSET_BASIS = 0xcbf29ce484222325; // FNV offset basis
    private const ulong FNV_PRIME = 0x100000001b3; // FNV prime

    // FNV-1a hash function to create a deterministic hash from a string input, which can be used to generate a seed from a string
    private int Hash(int seed, string input)
    {
        ulong hash = FNV_OFFSET_BASIS;

        // Taking base seed into account
        hash ^= (ulong)seed;
        hash *= FNV_PRIME;

        byte[] data = Encoding.UTF8.GetBytes(input);
        foreach (byte b in data)
        {
            hash ^= b;
            hash *= FNV_PRIME;
        }

        return (int)hash;
    }
}
