using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyPartyDataBase _enemyPartyDataBase;
    private List<GameObject> _createdEnemies = new List<GameObject>();

    public List<EnemyData> GenerateEnemies(List<Path> paths, int maxEnemies, System.Random random, int minEnemies = 0)
    {
        List<EnemyData> generatedEnemies = new List<EnemyData>();

        int numEnemies = random.Next(minEnemies, maxEnemies + 1);
        var availablePaths = new List<Path>(paths);

        for (int i = 0; i < numEnemies; i++)
        {
            int pathIdx = random.Next(availablePaths.Count);
            Path path = availablePaths[pathIdx];

            availablePaths.RemoveAt(pathIdx);
            
            PartyInfo enemyParty = _enemyPartyDataBase.GetRandomParty(random);
            EnemyInfo enemyInfo = enemyParty.PartyMembers[0]; // Select prefab from first enemy of the party

            generatedEnemies.Add(new EnemyData(enemyInfo.OverworldPrefab, path));
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
