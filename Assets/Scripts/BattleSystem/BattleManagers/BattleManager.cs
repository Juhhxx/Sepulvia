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
    private SoulBurnProfile _activeSoulBurn;
    public event Action<int,int> OnSoulBurn;
    public void ChangeSoulBurn(SoulBurnProfile profile)
    {
        _activeSoulBurn = profile.Clone() as SoulBurnProfile;
        _activeSoulBurn.OnStartBattle();
        _activeSoulBurn.OnSoulBurn += OnSoulBurn;
        _timelineUIManager.AddBSoulBurnIndicator(_activeSoulBurn);
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

    private void Awake()
    {
        GameObject battleLossTransitionScreen = GameObject.FindGameObjectWithTag("BattleLossTransitionScreen");
        _uiManager.setBattleLossTransitionScreen(battleLossTransitionScreen);
        battleLossTransitionScreen.SetActive(false);
    }

    public BattlerController GetBattlerController(Character character)
    {

        return _battlerControllers.GetValueOrDefault(character, null);
    }
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
    public void ExecuteAction(BattleAction action)
    {
        OnActionExecuted?.Invoke(action);
    }

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
        _doRun = _battleResolver.CanRun(GetBattlerController(Player), _enemyParty);
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

    private WaitForDialogueEnd _wfd;
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
            c.RecoveryTime = 0;
        }

        _hasWinner = false;
        _doRun = false;

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();

        CurrentState = BattleState.SetUp;

        _pullManager.TogglePullUI(true);
        _pullManager.SetUp(enemyParty);

        // Pull Bar Events
        _pullManager.OnHeartEnd += Win;

        // Instantiate Variables
        _wfd = new WaitForDialogueEnd(_dialogueManager);

        // Set UI
        _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
        _uiManager.SetUpStanceBars(Player);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _inventoryUIManager.ResetInventory();

        // Temporary
        _timelineUIManager.ClearIndicators();
        _timelineUIManager.AddPartyTimelineIndicators(_playerParty, _enemyParty);

        SetUpTurnEvents();

        _timelineManager.SetTurn(0);
        _timelineManager.ToggleTurnCounting(true);

        ChangeSoulBurn(_soulBurn);
    }

    private void SetUpTurnEvents()
    {
        // Clear Previous Subscribers
        _timelineManager.OnTurnBegin = null;
        _timelineManager.OnTurnEnd = null;

        _timelineManager.OnTurnBegin += StartTurn;

        _timelineManager.OnTurnBegin += _timelineUIManager.UpdateTurnIndicator;

        _timelineManager.OnTurnEnd += () =>
        {
            _activeSoulBurn.PassTurn();
            _activeSoulBurn.CheckSoulBurn();
        };

        // Count Turns in Modifiers
        _timelineManager.OnTurnEnd += () =>
        {
            foreach (Character c in _battlersList)
            {
                GetBattlerController(c).StatusEffectManager.UpdateStatusEffects();
            }
        };

        _timelineManager.OnTurnEnd += () =>
        {
            GetBattlerController(Player).CountItemTurn();
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
                        GetBattlerController(Player).RequestAction();
                        GetBattlerController(Player).SetItem(stack);
                        _inventoryUIManager.HideInventory();
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

    public void LoseCheat()
    {
        _playerWon = false;
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
        CurrentState = BattleState.BattleEnd;

        _uiManager.HideFinalScreens();
        _dialogueManager.ClearDialogues();

        _uiManager.ClearCreatedObjects();
        _pullManager.ResetEvents();

        GetBattlerController(Player).ClearActions();
        GetBattlerController(Player).StatusEffectManager.ClearStatusEffects();

        _timelineManager.OnTurnBegin -= StartTurn;

        ClearBattlerControllers();

        OnBattleEnd?.Invoke();
    }

    private void ShowEnd()
    {
        Debug.Log("END BATTLE NOW");
        _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _pullManager.TogglePullUI(false);

        if (_playerWon)
        {
            StartCoroutine(_uiManager.ShowWinScreen());
            OnBattleWon?.Invoke();
        }
        else
        {
            StartCoroutine(_uiManager.ShowLossScreen());
            OnBattleLost.Invoke();
        }

    }

    public void DoAssimilation()
    {
        (List<ItemInfo> items, int essence) = _battleResolver.GiveRewards(_enemyParty, false);

        (Player as Player).Essence += essence;

        if (items.Count > 0)
        {
            foreach (ItemInfo i in items) Player.Inventory.AddItem(i);
        }

        _uiManager.DoDecisionHeartAssimilateAnim(() =>
        {
            _uiManager.ShowRewards(items, essence);
            _uiManager.ShowRewardsScreen();
        });
    }
    public void DoSpare()
    {
        (List<ItemInfo> items, int essence) = _battleResolver.GiveRewards(_enemyParty, true);

        (Player as Player).Essence += essence;

        if (items.Count > 0)
        {
            foreach (ItemInfo i in items) Player.Inventory.AddItem(i);
        }

        _uiManager.DoDecisionHeartSpareAnim(() =>
        {
            _uiManager.ShowRewards(items, essence);
            _uiManager.ShowRewardsScreen();
        });
    }

    private void StartTurn()
    {
        if (_doRun)
        {
            _timelineManager.ToggleTurnCounting(false);
            EndBattle();
            return;
        }

        if (!_hasWinner)
        {
            StartCoroutine(ResolveTurn());
        }
        else
        {
            _timelineManager.ToggleTurnCounting(false);
            ShowEnd();
        }
    }

    private IEnumerator ResolveTurn()
    {
        OrganizeBattlers();

        CurrentState = BattleState.BattleTurnBegin;

        _timelineManager.ToggleTurnCounting(false);

        foreach (var battler in _battlersList)
        {
            battler.RecoveryTime -= 1;
        }

        foreach (var battler in _battlersList)
        {
            Debug.Log($"TURN { _timelineManager.CurrentTurn} - {battler.Name} ({battler.RecoveryTime})", this);

            BattlerController controller = GetBattlerController(battler);
            Party oppositeParty = battler is Player ? EnemyParty : PlayerParty;

            if (battler.RecoveryTime <= 0)
            {
                foreach (PassiveEffect pe in battler.PassiveEffects)
                {
                    pe.PassiveEffectLogic.OnBeginTurnEffect(controller, this, pe);
                }
            }

            while (battler.RecoveryTime <= 0)
            {
                BattleAction action = null;

                if (!controller.HasActions())
                {
                    if (controller.IsPlayer())
                    {
                        CurrentState = BattleState.PlayerTurnBegin;

                        _dialogueManager.HideDialogue();
                        _uiManager.SetUIState(BattleUIManager.BattleUIState.Action);

                        controller.RequestAction();

                        while (!controller.HasActions())
                        {
                            if (CurrentState == BattleState.PlayerChooseTarget)
                            {
                                _uiManager.SetUIState(BattleUIManager.BattleUIState.Target);
                            }
                            yield return null;
                        }

                        _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
                        _inventoryUIManager.HideInventory();
                        CurrentState = BattleState.PlayerTurnEnd;
                    }
                    else
                    {
                        CurrentState = BattleState.EnemyTurnBegin;
                        
                        BattlerController[] playerControllers = GetBattlerControllers(PlayerParty.PartyMembers.ToArray());
                        BattlerController[] enemyControllers = GetBattlerControllers(EnemyParty.PartyMembers.ToArray());

                        controller.BattleAI.ChooseRandom(controller, enemyControllers, playerControllers, 
                                                    _pullManager.BarSections, _pullManager.CurrentHeartIndex);

                        yield return new WaitForSeconds(1f);
                        CurrentState = BattleState.EnemyTurnEnd;
                    }
                }

                action = controller.GetAction(_timelineManager.CurrentTurn);

                yield return new WaitUntil(() => action != null);
                
                OnActionExecuted?.Invoke(action);

                if (CurrentState == BattleState.PlayerChooseBar)
                {
                    _pullManager.ToggleBarButtons(true);
                    _uiManager.SetUIState(BattleUIManager.BattleUIState.SelectBar);

                    yield return new WaitUntil(() => !_battleResolver.ApplyingBarModifier);

                    _pullManager.ToggleBarButtons(false);
                    _uiManager.SetUIState(BattleUIManager.BattleUIState.None);
                }

                yield return new WaitUntil(() => !_battleResolver.PlayingMinigame);

                _dialogueManager.StartDialogues();

                yield return new WaitUntil(() => !_pullManager.IsMoving);

                float waitTime = 0.5f;

                if (action.Move != null) waitTime = action.Move.MoveWaitTime;

                yield return new WaitForSeconds(waitTime);

                yield return _wfd;

                if (_hasWinner || _doRun)
                {
                    _timelineManager.ToggleTurnCounting(true);
                    yield break;
                }
            }

            if (battler.RecoveryTime <= 0)
            {
                foreach (PassiveEffect pe in battler.PassiveEffects)
                {
                    pe.PassiveEffectLogic.OnEndTurnEffect(controller, this, pe);
                }
            }
        }

        _timelineManager.ToggleTurnCounting(true);

        CurrentState = BattleState.BattleTurnEnd;
    }
}

