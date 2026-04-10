using TMPro;
using UnityEngine;
using System;

public class SeedManager : MonoBehaviourSingleton<SeedManager>
{
    [SerializeField] private int _seedMaxSize = 999999;
    private int _digits = 0;

    [SerializeField] private int _seed;
    public int Seed => _seed;

    [SerializeField] private TMP_InputField _seedInputField;

    private System.Random _random;

    private void Awake()
    {
        base.SingletonCheck(this);

        _digits = _seedMaxSize.ToString().Length;
    }

    public void SetSeed()
    {
        if (string.IsNullOrEmpty(_seedInputField.text))
        {
            GenerateSeed();
            return;
        }
        
        if (int.TryParse(_seedInputField.text, out int newSeed))
        {
            if (newSeed < 0 || newSeed > _seedMaxSize)
            {
                Debug.LogWarning($"Seed must be between 0 and {_seedMaxSize}");
                return;
            }
            _seed = newSeed;
        }
        else
        {
            Debug.LogWarning("Invalid seed input. Please enter a valid integer.");
        }
    }

    private void GenerateSeed()
    {
        _seed = UnityEngine.Random.Range(0, _seedMaxSize);
    }
}
