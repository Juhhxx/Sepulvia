using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;
    [SerializeField] private TimelineManager _timelineManager;
    [SerializeField] private TimelineUIManager _timelineUIManager;
    [SerializeField] private InventoryUIManager _inventoryUIManager;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private DialogueManager _dialogueManager;

    [Space(10)]
    [Header("Battlers Info")]
    [Space(5)]
    [SerializeField] private PlayerParty _playerParty;
    [SerializeField] private EnemyParty _enemyParty;

    public Character Player => _playerParty.Player;
    public PlayerParty PlayerParty => _playerParty;
    public EnemyParty EnemyParty => _enemyParty;
    
    private List<Character> _battlersList;
    private Dictionary<Character, BattlerController> _battlerControllers;
    public BattlerController GetBattlerController(Character character) => _battlerControllers[character];
    public void RegisterBattlerController(Character character, BattlerController controller)
    {
        Debug.Log($"Registering Battler Controller for {character.Name}", this);
        controller.SetUp(character, this);

        if (_battlerControllers == null)
            _battlerControllers = new Dictionary<Character, BattlerController>();

        if (!_battlerControllers.ContainsKey(character))
            _battlerControllers.Add(character, controller);
    }
    private void ClearBattlerControllers()
    {
        if (_battlerControllers != null)
            _battlerControllers.Clear();
    }


    [SerializeField, ReadOnly] private List<BattleAction> _actionList = new List<BattleAction>();

    
    public event Action<BattleAction> OnActionExecuted;

    // Battler Order
    
    private void OrganizeBattlers()
    {
        _battlersList = _enemyParty.PartyMembers.Concat(new List<Character> { Player }).ToList();

        _battlersList = _battlersList
                        .OrderByDescending(b => b.Speed)
                        .ToList();

        PrintBattlerOrder();
    }

    private void PrintBattlerOrder()
    {
        string order = "BATTLE ORDER: ";

        foreach (var battler in _battlersList)
        {
            order += battler.Name + $"({battler.Speed})" + " -> ";
        }

        Debug.Log(order, this);
    }

    // Battle States
    public enum BattleState
    {
        None,
        SetUp,
        BattleTurnBegin,
        PlayerTurnBegin,
        PlayerChooseTarget,
        PlayerChooseBar,
        PlayerTurnEnd,
        EnemyTurnBegin,
        EnemyTurnEnd,
        BattleTurnEnd,
        BattleEnd
    }

    private BattleState _currentState = BattleState.None;
    public BattleState CurrentState
    {
        get => _currentState;
        set
        {
            if (value != _currentState)
            {
                OnBattleStateChanged?.Invoke(value);

                Debug.Log($"Battle State Changed: {_currentState} -> {value}", this);
            }
            _currentState = value;
        }
    }
    public event Action<BattleState> OnBattleStateChanged;

    // Events
    public event Action OnBattleEnd;
    public UnityEvent OnBattleWon;
    public UnityEvent OnBattleLost;

    // Run logic
    public void Run()
    {
        bool result = _battleResolver.CanRun(Player, _enemyParty);

        if (result)
        {
            EndBattle();
        }
    }

    // Battle logic
    private bool _playerWon = false;
    private bool _hasWinner = false;
    private bool _doRun = false;

    public void StartBattle(PlayerParty playerParty, EnemyParty enemyParty)
    {
        _playerParty = playerParty;
        _enemyParty = enemyParty;

        _battlersList = new List<Character>();

        _battlersList.Add(Player);
        _battlersList.AddRange(enemyParty.PartyMembers);

        Player.ResetMoveCooldowns();
        Player.CurrentStance = 0;

        foreach (Character c in _enemyParty.PartyMembers)
        {
            c.ResetMoveCooldowns();
            c.CurrentStance = 0;
        }

        _hasWinner = false;
        _doRun = false;

        _currentState = BattleState.SetUp;

        _uiManager.ClearCreatedObjects();

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();

        _pullManager.TogglePullUI(true);
        _pullManager.SetUp(enemyParty);
        _pullManager.ResetEvents();

        // Pull Bar Events
        // _pullManager.OnSelectBar += (int index) => AddActionPlayer(Player.MoveSet[3], null);
        _pullManager.OnHeartEnd += Win;

        // On Action Executed Events
        OnActionExecuted += (_) => _dialogueManager.StartDialogues();

        // Instantiate Variables
        // _wfd = new WaitForDialogueEnd(_dialogueManager);
        _actionList = new List<BattleAction>();

        // Set Number of Battlers
        // _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        // Set UI
        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleTargetButtons(false);
        _uiManager.ToggleActionButtons(false);
        _uiManager.ToggleMoveInfo(false);
        _uiManager.SetUpStanceBars(Player);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _inventoryUIManager.ResetInventory();

        SetUpTurnEvents();

        _timelineManager.SetTurn(0);
        _timelineManager.ToggleTurnCounting(true);
    }

    private void SetUpTurnEvents()
    {
        // Clear Previous Subscribers
        _timelineManager.OnTurnBegin = null;
        _timelineManager.OnTurnEnd = null;

        _timelineManager.OnTurnBegin += StartTurn;

        // Count Turns in Modifiers and Check Them
        // _timelineManager.OnTurnEnd += () =>
        // {
        //     Player.CheckModifier();

        //     foreach (Character e in _enemyParty.PartyMembers)
        //     {
        //         e.CheckModifier();
        //     }
        //     Debug.Log("DID MODIFIERS COUNT");
        // };

        // Check Bar Modifiers
        _timelineManager.OnTurnEnd += _pullManager.CheckBarModifiers;

        // Count Turns in Moves
        _timelineManager.OnTurnEnd += () =>
        {
            foreach (Move m in Player.MoveSet) m.TurnPassed();

            foreach (Character c in _enemyParty.PartyMembers)
            {
                foreach (Move m in c.MoveSet) m.TurnPassed();
            }
            Debug.Log("DID MOVE COUNT");
        };
    }

    // Inventory UI
    public void SetUpInventoryButtons()
    {
        _inventoryUIManager.ShowInventory();

        var invButtons = _inventoryUIManager.GetItemButtons();

        for (int i = 0; i < invButtons.Count; i++)
        {
            ItemStack stack = (i < Player.Inventory.ItemSlots.Count) ? Player.Inventory.ItemSlots[i] : null;
            
            if (stack != null)
            {
                if (stack.Item.CanBeUsedInBattle)
                {
                    invButtons[i].enabled = true;
                    invButtons[i].onClick.RemoveAllListeners();
                    invButtons[i].onClick.AddListener(() =>
                    {
                        GetBattlerController(Player).SetItem(stack.Item);
                        Player.Inventory.RemoveItem(stack);
                    });
                }
                else
                {
                    invButtons[i].enabled = false;
                    Color c = Color.white;
                    c.a = 0.35f;
                    invButtons[i].transform.GetChild(0).GetComponent<Image>().color = c;
                }
            
                invButtons[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(stack.Item, _inventoryUIManager.ToggleItemInfo);
            }
            else
            {
                invButtons[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(_inventoryUIManager.ToggleItemInfo);
            }

        }

        invButtons = _inventoryUIManager.GetEquipmentButtons();

        for (int i = 0; i < invButtons.Count; i++)
        {
            ItemInfo item = (i < Player.Inventory.EquipmentSlots.Count) ? Player.Inventory.EquipmentSlots[i] : null;
            
            if (item != null)
            {
                invButtons[i].enabled = false;
                Color c = Color.white;
                c.a = 0.35f;
                invButtons[i].transform.GetChild(0).GetComponent<Image>().color = c;
            
                invButtons[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(item, _inventoryUIManager.ToggleItemInfo);
            }
            else
            {
                invButtons[i]?.GetComponent<ItemHoverInfo>()
                .SetUpHover(_inventoryUIManager.ToggleItemInfo);
            }

        }

    }

    public void WinCheat()
    {
        _playerWon = true;
        _hasWinner = true;
    }

    // Win Logic
    private void Win(bool playerWon)
    {
        Debug.Log($"WINNER IS PLAYER? {playerWon}");
        _playerWon = playerWon;
        _hasWinner = true;
    }

    // End Battle logic, Assimilation and Sparing
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void EndBattle()
    {
        _uiManager.HideFinalScreens();
        _dialogueManager.HideDialogue();

        Player.ResetModifiers();

        _timelineManager.ToggleTurnCounting(false);
        _timelineManager.OnTurnBegin -= StartTurn;

        ClearBattlerControllers();

        OnBattleEnd?.Invoke();
    }

    private void StartTurn()
    {
        if (_doRun) EndBattle();

        if (!_hasWinner)
        {
            StartCoroutine(ResolveTurn());
        }
        else
        {
            // ShowEnd();
        }
    }

    private IEnumerator ResolveTurn()
    {
        OrganizeBattlers();

        _currentState = BattleState.BattleTurnBegin;

        _timelineManager.ToggleTurnCounting(false);

        foreach (var battler in _battlersList)
        {
            battler.RecoveryTime -= 1;
        }

        _timelineUIManager.UpdateTimelineIndicators(Player.RecoveryTime, _enemyParty.PartyMembers[0].RecoveryTime);

        foreach (var battler in _battlersList)
        {
            Debug.Log($"TURN { _timelineManager.CurrentTurn} - {battler.Name} ({battler.RecoveryTime})", this);

            if (battler.RecoveryTime <= 0)
            {
                BattleAction action = null;

                if (!GetBattlerController(battler).HasActions())
                {
                    if (GetBattlerController(battler).IsPlayer())
                    {
                        _currentState = BattleState.PlayerTurnBegin;

                        _dialogueManager.HideDialogue();
                        _uiManager.ToggleActionButtons(true);

                        GetBattlerController(battler).RequestAction();

                        yield return new WaitUntil(() => _currentState == BattleState.PlayerChooseTarget);

                        _uiManager.ToggleMoveButtons(false);
                        _uiManager.ToggleTargetButtons(true);

                        yield return new WaitUntil(() => GetBattlerController(battler).HasActions());

                        _uiManager.ToggleTargetButtons(false);
                        _uiManager.ToggleActionButtons(false);
                        _uiManager.ToggleMoveInfo(false);
                        _uiManager.UpdateStanceBars(Player);
                        _inventoryUIManager.HideInventory();
                        _currentState = BattleState.PlayerTurnEnd;
                    }
                    else
                    {
                        _currentState = BattleState.EnemyTurnBegin;

                        var tmp = (battler as Enemy).BattleAI.ChooseRandom(_playerParty, _pullManager.BarSections, _pullManager.CurrentHeartIndex);

                        GetBattlerController(battler).QueueAction(tmp);

                        _currentState = BattleState.EnemyTurnEnd;
                    }
                }

                action = GetBattlerController(battler).GetAction();

                yield return new WaitUntil(() => action != null);
                
                OnActionExecuted?.Invoke(action);

                yield return new WaitUntil(() => !_pullManager.IsMoving);

                if (_hasWinner || _doRun) yield break; 
            }

            _timelineUIManager.UpdateTimelineIndicators(Player.RecoveryTime, _enemyParty.PartyMembers[0].RecoveryTime);
        }

        _timelineManager.ToggleTurnCounting(true);

        _currentState = BattleState.BattleTurnEnd;
    }
}