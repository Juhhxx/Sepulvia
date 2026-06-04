using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;
    [SerializeField] private InventoryUIManager _inventoryUIManager;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private DialogueManager _dialogueManager;

    [Space(10)]
    [Header("Battlers Info")]
    [Space(5)]
    [SerializeField] private PlayerParty _playerParty;
    [SerializeField] private EnemyParty _enemyParty;

    private Character Player => _playerParty.Player;
    
    private int _numberOfBattlers;

    // Battle Actions
    [SerializeField, ReadOnly] private List<BattleAction> _actionList;

    public enum ActionType { Move, Item, Run, Empty }

    [Serializable]
    public class BattleAction
    {
        [field: SerializeField] public Character Character { get; private set; }
        [field: SerializeField] public ActionType Type { get; private set;}
        [field: SerializeField] public Move Move { get; private set; }
        [field: SerializeField] public ItemInfo Item { get; private set;}
        [field: SerializeField] public int Priority { get; private set; }

        public BattleAction(Character character, Move move)
        {
            Character = character;
            Type = ActionType.Move;
            Move = move;
            Item = null;
            Priority = Move.PriorityLevel;
        }

        public BattleAction(Character character, ItemInfo item)
        {
            Character = character;
            Type = ActionType.Item;
            Move = null;
            Item = item;
            Priority = 5;
        }

        public BattleAction(Character character, bool isRun = false)
        {
            Character = character;
            Type = isRun ? ActionType.Run : ActionType.Empty;
            Move = null;
            Item = null;
            Priority = isRun ? 8 : 7;
        }
    }

    public void AddAction(Character character, bool isRun = false)
    {
        var action = new BattleAction(character, isRun);
        _actionList.Add(action);
    }
    public void AddAction(Character character, Move move)
    {
        var action = new BattleAction(character, move);
        _actionList.Add(action);
    }
    public void AddAction(Character character, ItemInfo item)
    {
        var action = new BattleAction(character, item);
        _actionList.Add(action);
    }

    public event Action<PlayerParty, EnemyParty> OnActionExecuted;

    private void ExecuteAction(BattleAction action)
    {
        switch (action.Type)
        {
            case ActionType.Move:

                Party party = action.Character is Player ? _enemyParty : _playerParty;
                // var target = ChooseTarget(party);

                action.Move.UsedMove();

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Move.Name}.");
                _battleResolver.DoMove(action.Move, action.Character, party);
                break;

            case ActionType.Item:

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Item.Name}.");
                _battleResolver.UseItem(action.Item, action.Character);
                break;

            case ActionType.Run:

                _dialogueManager.AddDialogue($"{Player.Name} tried to run.");
                Run();
                break;

            case ActionType.Empty:

                _dialogueManager.AddDialogue($"{action.Character.Name} lost their stance and took a turn to recover their balance.");
                action.Character.CurrentStance = action.Character.MaxStance;
                break;
        }
    }
    private void OrganizeActions()
    {
        Debug.Log($"PLAYER SPEED: {Player.Speed} ENEMY SPEED: {_enemyParty.PartyMembers[0].Speed}");

        _actionList = _actionList
                        .OrderByDescending(a => a.Priority)
                        .ThenByDescending(a => a.Character.Speed)
                        .ToList();

        Debug.Log("ACTION ORDER:");
        for (int i = 0; i < _actionList.Count; i++)
        {
            Debug.Log($"{_actionList[i].Character.Name}, Speed: {_actionList[i].Character.Speed}, Priority: {_actionList[i].Priority}");
        }
    }

    // Bar Modifiers
    private Move _barModifierMove;
    private void AddBarModifier(int index)
    {
        _barModifierMove.SetBarSection(index);
        AddAction(Player, _barModifierMove);

        _uiManager.ToggleSelecBar(false);
        _pullManager.ToggleBarButtons(false);
    }

    // Battle Turns
    public event Action OnBeginTurn;
    public event Action OnEndTurn;
    
    private int _currentTurn;
    public int CurrentTurn { get; set; }

    // Turn Events
    private void SetUpTurnEvents()
    {
        // Clear Previous Subscribers
        OnBeginTurn = null;
        OnEndTurn = null;

        // Count turns
        OnBeginTurn += () =>
        {
            CurrentTurn++;
            Debug.Log($"TURN {CurrentTurn} START");
        };

        // Make Stance Recovering
        OnBeginTurn += () =>
        {
            if (CurrentTurn == 0) return;

            if (Player.CurrentStance > 0) Player.CurrentStance += Player.StanceRecover;

            foreach (Character e in _enemyParty.PartyMembers)
            {
                if (e.CurrentStance > 0) e.CurrentStance += e.StanceRecover;
            }

            _uiManager.UpdateStanceBars(_playerParty, _enemyParty);
            Debug.Log("DID STANCE RECOVER");
        };

        // Make Enemies Choose Action Every Turn
        foreach (Enemy enemy in _enemyParty.PartyMembers)
        {
            OnBeginTurn += () =>
            {
                if (enemy.CurrentStance > 0)
                {
                    Move m = enemy.BattleAI.ChooseRandom();
                    AddAction(enemy, m);
                }
                Debug.Log("DID ENEMY AI");
            };
        }

        // Count Turns in Modifiers and Check Them
        OnEndTurn += () =>
        {
            Player.CheckModifier();

            foreach (Character e in _enemyParty.PartyMembers)
            {
                e.CheckModifier();
            }
            Debug.Log("DID MODIFIERS COUNT");
        };

        // Check Bar Modifiers
        OnEndTurn += _pullManager.CheckBarModifiers;

        // Count Turns in Moves
        OnEndTurn += () =>
        {
            foreach (Move m in Player.MoveSet) m.TurnPassed();

            foreach (Character c in _enemyParty.PartyMembers)
            {
                foreach (Move m in c.MoveSet) m.TurnPassed();
            }
            Debug.Log("DID MOVE COUNT");
        };
    }

    // Turn Losses
    private void VerifyTurnLosses()
    {
        // If stance = 0, character loses turn
        if (Player.CurrentStance == 0) AddAction(Player);

        foreach (Character e in _enemyParty.PartyMembers)
        {
            if (e.CurrentStance == 0) AddAction(e);
        }
    }

    // Other Variables
    private WaitForDialogueEnd _wfd;

    
    private bool _hasWinner = false;
    private bool _playerWon = false;

    public event Action OnBattleEnd;
    public UnityEvent OnBattleWon;
    public UnityEvent OnBattleLost;

    private void Start()
    {
// #if UNITY_EDITOR
//         if (_playerParty != null && _enemyParty != null) StartBattle(_playerParty, _enemyParty);
// #endif
    }

    public void StartBattle(PlayerParty playerParty, EnemyParty enemyParty)
    {
        _playerParty = playerParty;
        _enemyParty = enemyParty;

        Player.ResetMoveCooldowns();

        foreach (Character c in _enemyParty.PartyMembers)
        {
            c.ResetMoveCooldowns();
        }

        _hasWinner = false;
        _doRun = false;

        _uiManager.ClearCreatedObjects();

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();

        _pullManager.TogglePullUI(true);
        _pullManager.SetUp(enemyParty);
        _pullManager.ResetEvents();

        // Pull Bar Events
        _pullManager.OnSelectBar += AddBarModifier;
        _pullManager.OnHeartEnd += Win;

        // On Action Executed Events
        OnActionExecuted += _uiManager.UpdateStanceBars;
        OnActionExecuted += _uiManager.UpdateStatModifierDisplay;
        OnActionExecuted += (_,_) => _dialogueManager.StartDialogues();

        // Instantiate Variables
        _wfd = new WaitForDialogueEnd(_dialogueManager);
        _actionList = new List<BattleAction>();

        // Set Number of Battlers
        _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        // Set UI
        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleActionButtons(true);
        _uiManager.ToggleMoveInfo(false);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _inventoryUIManager.ResetInventory();

        SetUpTurnEvents();

        SetUpBattleUI();

        StopAllCoroutines();
        StartCoroutine(BattleLoopCR());
    }

    // Run logic
    public void DoRun() => AddAction(Player, true);
    bool _doRun = false;
    public void Run()
    {
        bool result = _battleResolver.CanRun(Player, _enemyParty);

        _doRun = result;
    }

    // Battle UI
    private void SetUpBattleUI()
    {
        _uiManager.SetUpStanceBars(_playerParty, _enemyParty);

        SetUpButtons();
    }

    private void SetUpButtons()
    {
        var moveButtons = _uiManager.GetMoveButtons();

        for (int i = 0; i < moveButtons.Count; i++)
        {
            Move move = (i < Player.MoveSet.Count) ? Player.MoveSet?[i] : null;

            if (move != null)
            {
                moveButtons[i].gameObject.SetActive(true);
                moveButtons[i].onClick.RemoveAllListeners();

                if (move.Type == MoveTypes.Modifier)
                {
                    _barModifierMove = move;
                    
                    moveButtons[i].onClick.AddListener(() => {
                        // Show Choose Bar UI
                        _uiManager.ToggleSelecBar(true);
                        _pullManager.ToggleBarButtons(true);
                    });
                }
                else
                    moveButtons[i].onClick.AddListener(() => AddAction(Player, move));

                _uiManager.SetUpButton(moveButtons[i], move);

                Debug.Log($"SET MOVE BUTON FOR {move.Name}");
            }
            else moveButtons[i].gameObject.SetActive(false);

        }
    }
    private void UpdateButtons()
    {
        var moveButtons = _uiManager.GetMoveButtons();

        for (int i = 0; i < moveButtons.Count; i++)
        {
            Move move = (i < Player.MoveSet.Count) ? Player.MoveSet?[i] : null;

            if (move != null)
            {
                _uiManager.UpdateButton(moveButtons[i],
                !move.CheckIfCooldown() && move.CheckStanceCost(Player));

                Debug.Log($"UPDATE MOVE BUTON FOR {move.Name}");
            }
        }
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
                        AddAction(Player, stack.Item);
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

    // Battle Loop
    private IEnumerator BattleLoopCR()
    {
        while (true)
        {
            VerifyTurnLosses();

            OnBeginTurn?.Invoke();

            UpdateButtons();
            _uiManager.UpdateStatModifierDisplay(_playerParty, _enemyParty);
            _uiManager.ShowTurnOrder(_playerParty, _enemyParty);

            yield return new WaitUntil(() => _actionList.Count == _numberOfBattlers);

            _uiManager.ToggleMoveButtons(false);
            _uiManager.ToggleActionButtons(false);
            _uiManager.ToggleMoveInfo(false);
            _uiManager.HideTurnOrder();
            _inventoryUIManager.HideInventory();

            // Little Delay Before Battle Starts
            yield return new WaitForSeconds(0.8f);

            OrganizeActions();

            foreach (BattleAction action in _actionList)
            {
                ExecuteAction(action);

                OnActionExecuted?.Invoke(_playerParty, _enemyParty);

                yield return new WaitUntil(() => !_pullManager.IsMoving);
                yield return _wfd;
                yield return new WaitForSeconds(0.5f);

                Debug.Log("FINISHED TURN");

                if (_hasWinner)
                {
                    Debug.Log("END BATTLE");
                    ShowEnd();
                    yield break;
                }

                if (_doRun)
                {
                    Debug.Log("END BATTLE");
                    EndBattle();
                    yield break;
                }
            }

            OnEndTurn?.Invoke();

            yield return DoEndTurnEffects();

            // Clear Actions List
            _actionList.Clear();
            _uiManager.ToggleActionButtons(true);
        }
    }

    public IEnumerator DoEndTurnEffects()
    {
        _pullManager.DoBarModifiers(BarModifierTrigger.OnEndTurn);

        OnActionExecuted?.Invoke(_playerParty, _enemyParty);
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_pullManager.IsMoving);
        yield return _wfd;
        yield return new WaitForSeconds(0.5f);
    }

    // Win Logic
    private void Win(bool playerWon)
    {
        Debug.Log($"WINNER IS PLAYER? {playerWon}");
        _playerWon = playerWon;
        _hasWinner = true;
    }

    // End Battle logic, Assimilation and Sparing
    private void ShowEnd()
    {
        Debug.Log("END BATTLE NOW");
        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleActionButtons(false);
        _uiManager.ToggleMoveInfo(false);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _pullManager.TogglePullUI(false);

        if (_playerWon)
        {
            _uiManager.ShowWinScreen();
            OnBattleWon?.Invoke();
        }
        else
        {
            _uiManager.ShowLoseScreen();
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

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void EndBattle()
    {
        _uiManager.HideFinalScreens();
        _dialogueManager.HideDialogue();

        Player.ResetModifiers();

        OnBattleEnd?.Invoke();
    }
    
}