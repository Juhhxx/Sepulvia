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

    [Space(10)]
    [Header("Soul Burn")]
    [Space(5)]
    [SerializeField] private SoulBurnProfile _soulBurn;
    public event Action<int,int> OnSoulBurn;
    public void ChangeSoulBurn(SoulBurnProfile profile)
    {
        _soulBurn = profile;
        _soulBurn.OnStartBattle();
        _soulBurn.OnSoulBurn += OnSoulBurn;
    }

    [Space(10)]
    [Header("Player Buttons")]
    [Space(5)]
    [SerializeField] private Button _moveButton;
    public Button MoveButton => _moveButton;
    [SerializeField] private Button _stanceMoveButton;
    public Button StanceMoveButton => _stanceMoveButton;
    [SerializeField] private Button _itemsButton;
    public Button ItemsButton => _itemsButton;
    [SerializeField] private Button _runButton;
    public Button RunButton => _runButton;

    public Character Player => _playerParty.Player;
    public PlayerParty PlayerParty => _playerParty;
    public EnemyParty EnemyParty => _enemyParty;
    
    private List<Character> _battlersList;
    private Dictionary<Character, BattlerController> _battlerControllers;
    public BattlerController GetBattlerController(Character character) => _battlerControllers[character];
    public BattlerController[] GetBattlerControllers(Character[] characters)
    {
        BattlerController[] controllers = new BattlerController[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            controllers[i] = GetBattlerController(characters[i]); 
        }

        return controllers;
    }
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
        BattleEnd0
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
        bool result = _battleResolver.CanRun(GetBattlerController(Player), _enemyParty);

        if (result)
        {
            EndBattle();
        }
    }

    // Battle logic
    private bool _playerWon = false;
    private bool _hasWinner = false;
    private bool _doRun = false;

    private void Start()
    {
        _moveButton.onClick.AddListener(() => _uiManager.SetUIState(BattleUIManager.BattleUIState.Move));
        _stanceMoveButton.onClick.AddListener(() => _uiManager.SetUIState(BattleUIManager.BattleUIState.StanceMove));
        _runButton.onClick.AddListener(() => GetBattlerController(Player).AddActionRun());
    }

    public void StartBattle(PlayerParty playerParty, EnemyParty enemyParty)
    {
        _playerParty = playerParty;
        _enemyParty = enemyParty;

        _battlersList = new List<Character>();

        _battlersList.Add(Player);
        _battlersList.AddRange(enemyParty.PartyMembers);

        foreach (Character c in _battlersList)
        {
            c.ResetMoveCooldowns();
            c.CurrentStance = 0;
        }

        _hasWinner = false;
        _doRun = false;

        _currentState = BattleState.SetUp;

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();

        _pullManager.TogglePullUI(true);
        _pullManager.SetUp(enemyParty);

        // Pull Bar Events
        _pullManager.OnHeartEnd += Win;

        // On Action Executed Events
        OnActionExecuted += (_) => _dialogueManager.StartDialogues();

        // Instantiate Variables
        // _wfd = new WaitForDialogueEnd(_dialogueManager);

        // Set UI
        _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
        _uiManager.SetUpStanceBars(Player);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _inventoryUIManager.ResetInventory();

        SetUpTurnEvents();

        _timelineManager.SetTurn(0);
        _timelineManager.ToggleTurnCounting(true);

        ChangeSoulBurn(_soulBurn);

        foreach (Character c in _battlersList)
        {
            foreach (PassiveEffect pe in c.PassiveEffects)
            {
                pe.PassiveEffectLogic.OnEnterBattleEffect(GetBattlerController(c), this, pe);
            }
        }
    }

    private void SetUpTurnEvents()
    {
        // Clear Previous Subscribers
        _timelineManager.OnTurnBegin = null;
        _timelineManager.OnTurnEnd = null;

        _timelineManager.OnTurnBegin += StartTurn;

        // Count Turns in Modifiers and Check Them
        _timelineManager.OnTurnEnd += () =>
        {
            foreach (Character c in _battlersList)
            {
                GetBattlerController(c).StatusEffectManager.UpdateStatusEffects();
            }
        };

        // Check Bar Modifiers
        _timelineManager.OnTurnEnd += _pullManager.CheckBarModifiers;

        // Count Turns in Moves
        _timelineManager.OnTurnEnd += () =>
        {
            foreach (Character c in _battlersList)
            {
                foreach (Move m in c.MoveSet) m.TurnPassed();
                foreach (Move m in c.StanceMoveSet) m.TurnPassed();
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
        Debug.Log("ENDING BATTLE");
        _uiManager.HideFinalScreens();
        _dialogueManager.HideDialogue();

        _uiManager.ClearCreatedObjects();
        _pullManager.ResetEvents();

        Player.ResetModifiers();

        _timelineManager.ToggleTurnCounting(false);
        _timelineManager.OnTurnBegin -= StartTurn;

        ClearBattlerControllers();

        OnBattleEnd?.Invoke();
    }

    private void StartTurn()
    {
        if (_doRun) EndBattle();

        if (_soulBurn.NextTurn == _timelineManager.CurrentTurn) _soulBurn.OnTurnReached();

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

            Party oppositeParty = battler is Player ? EnemyParty : PlayerParty;

            if (battler.RecoveryTime <= 0)
            {
                foreach (PassiveEffect pe in battler.PassiveEffects)
                {
                    pe.PassiveEffectLogic.OnBeginTurnEffect(GetBattlerControllers(oppositeParty.PartyMembers.ToArray()));
                }
            }

            while (battler.RecoveryTime <= 0)
            {
                BattlerController controller = GetBattlerController(battler);
                BattleAction action = null;

                if (!controller.HasActions())
                {
                    if (controller.IsPlayer())
                    {
                        _currentState = BattleState.PlayerTurnBegin;

                        _dialogueManager.HideDialogue();
                        _uiManager.SetUIState(BattleUIManager.BattleUIState.Action);

                        controller.RequestAction();

                        while (!controller.HasActions())
                        {
                            if (_currentState == BattleState.PlayerChooseTarget)
                            {
                                _uiManager.SetUIState(BattleUIManager.BattleUIState.Target);
                            }
                            yield return null;
                        }

                        _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
                        _uiManager.UpdateStanceBars(Player);
                        _inventoryUIManager.HideInventory();
                        _currentState = BattleState.PlayerTurnEnd;
                    }
                    else
                    {
                        _currentState = BattleState.EnemyTurnBegin;
                        
                        BattlerController[] playerControllers = _playerParty.PartyMembers.Select(c => GetBattlerController(c)).ToArray();

                        controller.BattleAI.ChooseRandom(controller, playerControllers, 
                                _pullManager.BarSections, _pullManager.CurrentHeartIndex);

                        _currentState = BattleState.EnemyTurnEnd;
                    }
                }

                action = controller.GetAction(_timelineManager.CurrentTurn);

                yield return new WaitUntil(() => action != null);
                
                OnActionExecuted?.Invoke(action);

                if (_currentState == BattleState.PlayerChooseBar)
                {
                    _pullManager.ToggleBarButtons(true);
                    _uiManager.SetUIState(BattleUIManager.BattleUIState.SelectBar);

                    yield return new WaitUntil(() => !_battleResolver.ApplyingBarModifier);

                    _pullManager.ToggleBarButtons(false);
                    _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
                }

                yield return new WaitUntil(() => !_battleResolver.PlayingMinigame);

                yield return new WaitUntil(() => !_pullManager.IsMoving);

                float waitTime = 0.5f;

                if (action.Move != null) waitTime = action.Move.MoveWaitTime;

                yield return new WaitForSeconds(waitTime);

                if (_hasWinner || _doRun) yield break; 
            }

            if (battler.RecoveryTime <= 0)
            {
                foreach (PassiveEffect pe in battler.PassiveEffects)
                {
                    pe.PassiveEffectLogic.OnEndTurnEffect(GetBattlerControllers(oppositeParty.PartyMembers.ToArray()));
                }
            }

            _timelineUIManager.UpdateTimelineIndicators(Player.RecoveryTime, _enemyParty.PartyMembers[0].RecoveryTime);
        }

        _timelineManager.ToggleTurnCounting(true);

        _currentState = BattleState.BattleTurnEnd;
    }
}

[Serializable]
public class SoulBurnProfile
{
    [SerializeField] private int _waitTimeTurns;
    public int WaitTimeTurns => _waitTimeTurns;

    [SerializeField] private int _waitTimeMinimum;
    public int WaitTimeMinimum => _waitTimeMinimum;

    [SerializeField] private float _waitTimeChangeRate;
    public float WaitTimeChangeRate => _waitTimeChangeRate;

    [SerializeField] private bool _doLeft;
    [SerializeField] private bool _doRight;
    public (bool, bool) DoLeftRight => (_doLeft, _doRight);

    [SerializeField] private int _amount;
    public int Amount => _amount;

    private int _nextTurn;
    public int NextTurn => _nextTurn;

    public event Action<int,int> OnSoulBurn; // ints to indicate how much burn on each side

    public void OnStartBattle()
    {
        _nextTurn = _waitTimeTurns;
    }

    public void OnTurnReached()
    {
        _waitTimeTurns = Mathf.FloorToInt(_waitTimeTurns * _waitTimeChangeRate);
        _waitTimeTurns = Mathf.Max(_waitTimeTurns, _waitTimeMinimum);
        _nextTurn += _waitTimeTurns;

        OnSoulBurn?.Invoke(_doLeft ? _amount : 0,
                            _doRight ? _amount : 0);
    }
}