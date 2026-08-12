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
    
    private List<Character> _battlersList;

    // Add Player Battle Actions
    [SerializeField, ReadOnly] private List<BattleAction> _actionList = new List<BattleAction>();

    public void AddActionPlayerRun()
    {
        var action = new BattleAction(Player);
        Player.QueueAction(action);
    }
    public void AddActionPlayer(Move move)
    {
        var action = new BattleAction(Player, move);
        Player.QueueAction(action);
    }
    private void AddActionPlayer(Move move, int index)
    {
        move.SetBarSection(index);

        var action = new BattleAction(Player, move);

        Player.QueueAction(action);
    }
    public void AddActionPlayer(ItemInfo item)
    {
        var action = new BattleAction(Player, item);
        Player.QueueAction(action);
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
        }
    }

    // Battler Order
    
    private void OrganizeBattlers()
    {
        _battlersList = _enemyParty.PartyMembers.Concat(new List<Character> { Player }).ToList();

        _battlersList = _battlersList
                        .OrderByDescending(b => b.Speed)
                        .ToList();
        
        Debug.LogWarning("BATTLER ORDER:");
        for (int i = 0; i < _battlersList.Count; i++)
        {
            Debug.LogWarning($"{_battlersList[i].Name}, Speed: {_battlersList[i].Speed}, Recovery: {_battlersList[i].RecoveryTime}");
        }
    }

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
        _pullManager.OnSelectBar += (int index) => AddActionPlayer(Player.MoveSet[3], index);
        _pullManager.OnHeartEnd += Win;

        // On Action Executed Events
        // OnActionExecuted += _uiManager.UpdateStanceBars;
        OnActionExecuted += _uiManager.UpdateStatModifierDisplay;
        OnActionExecuted += (_,_) => _dialogueManager.StartDialogues();

        // Instantiate Variables
        // _wfd = new WaitForDialogueEnd(_dialogueManager);
        _actionList = new List<BattleAction>();

        // Set Number of Battlers
        // _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        // Set UI
        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleActionButtons(false);
        _uiManager.ToggleMoveInfo(false);
        _dialogueManager.HideDialogue();
        _inventoryUIManager.HideInventory();
        _inventoryUIManager.ResetInventory();

        SetUpTurnEvents();

        SetUpBattleUI();

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
        _timelineManager.OnTurnEnd += () =>
        {
            Player.CheckModifier();

            foreach (Character e in _enemyParty.PartyMembers)
            {
                e.CheckModifier();
            }
            Debug.Log("DID MODIFIERS COUNT");
        };

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

    private void SetUpBattleUI()
    {
        // _uiManager.SetUpStanceBars(_playerParty, _enemyParty);

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
                    moveButtons[i].onClick.AddListener(() => {
                        // Show Choose Bar UI
                        _uiManager.ToggleSelecBar(true);
                        _pullManager.ToggleBarButtons(true);
                    });
                }
                else
                    moveButtons[i].onClick.AddListener(() => AddActionPlayer(move));

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
                        AddActionPlayer(stack.Item);
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
            MenuManager.Instance.ToggleGameOverMenu(true);
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

        _timelineManager.ToggleTurnCounting(false);
        _timelineManager.OnTurnBegin -= StartTurn;

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
            ShowEnd();
        }
    }

    private IEnumerator ResolveTurn()
    {
        OrganizeBattlers();
        
        _timelineManager.ToggleTurnCounting(false);

        foreach (var battler in _battlersList)
        {
            battler.RecoveryTime -= 1;
        }

        _timelineUIManager.UpdateTimelineIndicators(Player.RecoveryTime, _enemyParty.PartyMembers[0].RecoveryTime);

        foreach (var battler in _battlersList)
        {
            if (battler.RecoveryTime <= 0)
            {
                BattleAction action = null;

                if (!battler.HasActions())
                {
                    if (battler is Player)
                    {
                        UpdateButtons();
                        _uiManager.ToggleActionButtons(true);
                        _uiManager.ShowTurnOrder(_playerParty, _enemyParty);

                        yield return new WaitUntil(() => battler.HasActions());

                        _uiManager.ToggleMoveButtons(false);
                        _uiManager.ToggleActionButtons(false);
                        _uiManager.ToggleMoveInfo(false);
                        _inventoryUIManager.HideInventory();
                    }
                    else
                    {
                        var tmp = (battler as Enemy).BattleAI.ChooseRandom(_pullManager.BarSections, _pullManager.CurrentHeartIndex);

                        battler.QueueAction(tmp);
                    }
                }

                action = battler.GetAction();

                yield return new WaitUntil(() => action != null);
                
                ExecuteAction(action);

                yield return new WaitUntil(() => !_pullManager.IsMoving);

                if (_hasWinner || _doRun) yield break; 
            }

            _timelineUIManager.UpdateTimelineIndicators(Player.RecoveryTime, _enemyParty.PartyMembers[0].RecoveryTime);
        }

        _timelineManager.ToggleTurnCounting(true);
    }
}