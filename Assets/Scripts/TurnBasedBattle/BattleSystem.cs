using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
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

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;

    private const string ACTION_MESSAGE = " 's Action:";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyManager = GameObject.FindAnyObjectByType<PartyManager>();
        enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        AttackAction(allBattlers[0],allBattlers[1]);
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

    private void ShowBattleMenu()
    {
        //whos action it is
        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        //enableing our battle menu
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        //disable the battle menu
        battleMenu.SetActive(false);
        //set our enemy selection buttons
        SetEnemySelectionButtons();
        //enable our selection menu
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        //disable all of our buttons
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            enemySelectionButtons[i].SetActive(true); //enable buttons for each enemy
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;  //change the buttons text
        }
    }

    public void SelectEnemy(int currentEnemy)
    {
        //setting the current members target
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        //tell the battle system this member intends to attack
        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        //increment through our party members
        currentPlayer++;
        //if all players have selected an actiong, we ll start the battle
        if (currentPlayer >= playerBattlers.Count)
        {
            //StartBattle
            Debug.Log("Start the battle!");
            Debug.Log("We are attacking: " + allBattlers[currentPlayerEntity.Target].Name);
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            //show the battle menu for the next player
            ShowBattleMenu();
        }
    }
    
    private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget) //kind of a template for every single battle entity
    {
        //get damage
        int damage = currAttacker.Strength; //can sue an algorithm to scale
        //play the attack animation
        //currAttacker.BattleVisuals.PlayAttackAnimation();
        //dealing the damage
        currTarget.CurrHealth -= damage;
        //enemy play their hit animation
        //currTarget.battleVisuals.PlayHitAnimation();
        //update the UI
        currTarget.UpdateUI(); //remmeber this is still incomplete
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.Name, currTarget.Name, damage);
    }
}

[System.Serializable]
public class BattleEntities //create another class that will umbrella anything that enters our battle system
{                           //keeps everything compartimentalized without any fear of overwritting any of the other data
    public enum Action { Attack, Run} //add eventually other states over here associated with basic actions liek usign an item or even talk
    public Action BattleAction;

    public string Name;     //makes it easier to compare attributes - asking what battle entity turn it is than "has the partymember had his turn? No? then who took their last turn?" 
    public int CurrHealth;  //this also makes it easier when it comes to group effects/group attacks
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public int Target;

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

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        //battleVisuals.changeHealth(CurrHealth);
    }
}
