using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour, IRandom
{
    [SerializeField] private List<PartyInfo> _enemyPool;

    private void Start()
    {
        SeedManager.Instance.RegisterRandom(this, transform.GetPath());
    }

    // IRandom Implmentation
    private System.Random _random;
    public void InitializeRandom(int seed)
    {
        throw new System.NotImplementedException();
    }
}
