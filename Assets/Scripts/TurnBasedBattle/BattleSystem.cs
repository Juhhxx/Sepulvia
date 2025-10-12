using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

    [Header("Battle State")]
    [ReadOnly]
    [SerializeField] private BattleState state;

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
    private const int TURN_DURATION = 2;
    private const string WIN_MESSAGE = "Your party won the battle";
    private const string LOSE_MESSAGE = "Your party lost the battle";
    private const string OVERWORLD_SCENE = "OverworldTestScene";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyManager = GameObject.FindAnyObjectByType<PartyManager>();
        enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
    }

    private IEnumerator BattleRoutine()  //THIS SI THE MAIN BATTLE COROUTINE
    {
        //enemy selectionMenu disabled
        enemySelectionMenu.SetActive(false);
        //change our state to the battle state
        state = BattleState.Battle;
        //enable our bottom text
        bottomTextPopUp.SetActive(true);

        //loop through all our battlers
        //do their appropriate action
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (state == BattleState.Battle)
            {
                switch (allBattlers[i].BattleAction)  //a case for each possible action tot ake, add mroe as action are created
                {
                    case BattleEntities.Action.Attack:
                        //do the attack
                        yield return StartCoroutine(AttackRoutine(i));
                        break;

                    case BattleEntities.Action.Run:
                        //Run
                        break;

                    default:
                        Debug.Log("Error - Incorrect battle action");
                        break;
                }
            }
        }

        //if we ahvent  won or lost repeat the loop by opening the battle menu
        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }

        yield return null;
    }

    private IEnumerator AttackRoutine(int i)
    {
        if (allBattlers[i].IsPlayer == true) //confirm if it is players' turn
        {
            BattleEntities currAttacker = allBattlers[i];
            if (allBattlers[currAttacker.Target].IsPlayer == true || currAttacker.Target <= allBattlers.Count) //if somehow we slip outside the bounds of the list we just pick a random enemy
            {
                currAttacker.SetTarget(GetRandomEnemy());
            }
            BattleEntities currTarget = allBattlers[currAttacker.Target];

            AttackAction(currAttacker, currTarget); //attack selected enemy (attack action)

            yield return new WaitForSeconds(TURN_DURATION); //wait a few secs

            //kil the enemy?
            if (currTarget.CurrHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION); //wait a few secs
                enemyBattlers.Remove(currTarget);
                allBattlers.Remove(currTarget);

                if (enemyBattlers.Count == 0) //if no enemies remain we win the battle
                {
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION); //wait a few secs
                    Debug.Log("Go back to Overworld Scene"); //Switch scenes to overworld
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                }
            }
        }

        //enemy's turn
        if(allBattlers[i].IsPlayer == false)
        {
            BattleEntities currAttacker = allBattlers[i];
            currAttacker.SetTarget(GetRandomPartyMember()); //this is the enemy picking a party member to atatck at random
            BattleEntities currTarget = allBattlers[currAttacker.Target];
            //get random party member (taregt)

            AttackAction(currAttacker, currTarget);//attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION); //wait a few secs
            if(currTarget.CurrHealth <= 0) //kill party member?
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION); //wait a few secs
                playerBattlers.Remove(currTarget);
                allBattlers.Remove(currTarget);

                //if no party emmbers remain we lost the battle
                if (playerBattlers.Count <= 0)
                {
                    state = BattleState.Lost;
                    bottomText.text = LOSE_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION); //wait a few secs
                    Debug.Log("Game Over");
                }
            }
            
        }
        
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
            StartCoroutine(BattleRoutine());
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
        SaveHealth();
    }

    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>();
        //create a temporary list of type int (index)
        //find all the party members -> add to our list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == true)
            {
                partyMembers.Add(i);
            }
        }
        //return the random party emmber
        return partyMembers[Random.Range(0, partyMembers.Count)];
    }

    private int GetRandomEnemy()
    {
        List<int> enemies = new List<int>();
        //create a temporary list of type int (index)
        //find all the party members -> add to our list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == false)
            {
                enemies.Add(i);
            }
        }
        //return the random party emmber
        return enemies[Random.Range(0, enemies.Count)];
    }
    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)  //run through all party members
        {
            partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);  //save their current health
        }
    }
}

[System.Serializable]
public class BattleEntities //create another class that will umbrella anything that enters our battle system
{                           //keeps everything compartimentalized without any fear of overwritting any of the other data
    public enum Action { Attack, Run } //add eventually other states over here associated with basic actions liek usign an item or even talk
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
