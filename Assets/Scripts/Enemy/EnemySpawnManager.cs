using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour, IRandom
{
    [SerializeField] private Transform _roomTransform;
    [SerializeField] private List<GameObject> _enemyPool;

    private void Start()
    {
        SeedManager.Instance.RegisterRandom(this, transform.GetPath());
    }

    public void SpawnEnemies(List<Path> paths, int maxEnemies)
    {
        int numEnemies = _random.Next(maxEnemies);
        var availablePaths = new List<Path>(paths);

        for (int i = 0; i < numEnemies; i++)
        {
            int pathIdx = _random.Next(availablePaths.Count);
            Path path = availablePaths[pathIdx];
            
            int enemyIdx = _random.Next(_enemyPool.Count);
            GameObject enemy = Instantiate(_enemyPool[enemyIdx], _roomTransform);

            EnemyBrain enemyBrain = enemy.GetComponent<EnemyBrain>();
            enemyBrain.SetPath(path);
        }
    }

    // IRandom Implmentation
    private System.Random _random;
    public void InitializeRandom(int seed)
    {
        _random = new System.Random(seed);
    }
}
