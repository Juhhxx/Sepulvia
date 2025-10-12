using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] private Encounter[] enemiesInScene; //Given that we have different ambients(scenes in this context), this array holds whcih enemies can show up in that particular biome per say
    [SerializeField] private int maxNumEnemies; //max numb of enemies that can be egnerated per encounter

    private EnemyManager enemyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        enemyManager.generateEnemiesByEncounter(enemiesInScene, maxNumEnemies);
    }

}
[System.Serializable]
public class Encounter
{
    public EnemyInfo Enemy;
    public int levelMin;
    public int LevelMax;
}
