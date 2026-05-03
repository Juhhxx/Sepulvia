using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemyPool;
    private List<GameObject> _createdEnemies = new List<GameObject>();

    public List<EnemyData> GenerateEnemies(List<Path> paths, int maxEnemies, System.Random random)
    {
        List<EnemyData> generatedEnemies = new List<EnemyData>();

        int numEnemies = random.Next(1, maxEnemies + 1);
        var availablePaths = new List<Path>(paths);

        for (int i = 0; i < numEnemies; i++)
        {
            int pathIdx = random.Next(availablePaths.Count);
            Path path = availablePaths[pathIdx];
            
            int enemyIdx = random.Next(_enemyPool.Count);
            GameObject enemy = _enemyPool[enemyIdx];

            generatedEnemies.Add(new EnemyData(enemy, path));
        }

        return generatedEnemies;
    }

    public void SpawnEnemies(List<EnemyData> enemyDataList, Transform enemiesParent)
    {
        foreach (GameObject go in _createdEnemies) Destroy(go);
        _createdEnemies.Clear();

        foreach (EnemyData e in enemyDataList)
        {
            EnemyBrain enemyBrain = Instantiate(e.Enemy, enemiesParent).GetComponent<EnemyBrain>();

            enemyBrain.SetPath(e.Path);

            _createdEnemies.Add(enemyBrain.gameObject);
        }
    }
}
