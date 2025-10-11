using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BattleSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [Header("Battlers")]
    [ReadOnly]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [ReadOnly]
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [ReadOnly]
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyManager = GameObject.FindAnyObjectByType<PartyManager>();
        enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        //get current party
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities(); //create battle entities for our party

            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth, //assign the values
            currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrentEnemies();

        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities(); //create battle entities for our party

            tempEntity.SetEntityValues(currentEnemies[i].Enemyname, currentEnemies[i].CurrHealth, currentEnemies[i].MaxHealth, //assign the values
            currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);

            allBattlers.Add(tempEntity); //add to the list of all battlers
            enemyBattlers.Add(tempEntity); //add to the list of enemies
        }
    }

}

[System.Serializable]
public class BattleEntities //create another class that will umbrella anything that enters our battle system
{                           //keeps everything compartimentalized without any fear of overwritting any of the other data
    public string Name;     //makes it easier to compare attributes - asking what battle entity turn it is than "has the partymember had his turn? No? then who took their last turn?" 
    public int CurrHealth;  //this also makes it easier when it comes to group effects/group attacks
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;

    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        IsPlayer = isPlayer;
    }
}
