using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private DialogueManager _dialogueManager;

    [Space(10)]
    [Header("Battlers Info")]
    [Space(5)]
    [SerializeField, Expandable] private PartyInfo _playerParty;
    [SerializeField, Expandable] private PartyInfo _enemyParty;

    private CharacterInfo Player => _playerParty.PartyMembers[0];
    
    private int _numberOfBattlers;

    // Battle Action Variables
    [SerializeField, ReadOnly] private List<BattleAction> _actionList;

    public enum ActionType { Move, Item, Empty }

    [Serializable]
    public class BattleAction
    {
        [field: SerializeField] public CharacterInfo Character { get; private set; }
        [field: SerializeField] public ActionType Type { get; private set;}
        [field: SerializeField] public MoveInfo Move { get; private set; }
        [field: SerializeField] public ItemInfo Item { get; private set;}

        public BattleAction(CharacterInfo character, MoveInfo move)
        {
            Character = character;
            Type = ActionType.Move;
            Move = move;
            Item = null;
        }

        public BattleAction(CharacterInfo character, ItemInfo item)
        {
            Character = character;
            Type = ActionType.Item;
            Move = null;
            Item = item;
        }

        public BattleAction(CharacterInfo character)
        {
            Character = character;
            Type = ActionType.Empty;
            Move = null;
            Item = null;
        }
    }

    // Battle Turn Variables
    public event Action OnTurnPassed;
    private int _currentTurn;
    public int CurrentTurn
    {
        get => _currentTurn;

        private set
        {
            _currentTurn = value;
            OnTurnPassed?.Invoke();
        }
    }

    private WaitForDialogueEnd _wfd;

    private bool _selectingBar;
    private bool _hasWinner = false;
    private bool _playerWon;

    private void Start()
    {
// #if UNITY_EDITOR
//         if (_playerParty != null && _enemyParty != null) StartBattle(_playerParty, _enemyParty);
// #endif
    }

    public void StartBattle(PartyInfo playerParty, PartyInfo enemyParty)
    {
        _playerParty = playerParty.Instantiate();
        _enemyParty = enemyParty.Instantiate();

        _hasWinner = false;

        _uiManager.ClearCreatedObjects();

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();

        _pullManager.SpawnHeart();
        _pullManager.SpawnBarSections((_enemyParty.PartySize * 5) + 5);
        _pullManager.SetHeartInMiddle();
        _pullManager.ResetEvents();

        _pullManager.OnSelectBar += () => _selectingBar = false;
        _pullManager.OnHeartEnd += Win;

        _wfd = new WaitForDialogueEnd(_dialogueManager);
        _actionList = new List<BattleAction>();

        _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleActionButtons(true);
        _uiManager.ToggleMoveInfo(false);
        _dialogueManager.HideDialogue();
        _inventoryManager.HideInventory();

        SetUpNewTurnEvents();

        SetUpBattleUI();

        StopAllCoroutines();
        StartCoroutine(BattleLoopCR());
    }

    public void Run() => StartCoroutine(RunCR());

    public IEnumerator RunCR()
    {
        _uiManager.ToggleActionButtons(false);
        _dialogueManager.StartDialogues($"{Player.Name} ran.");

        yield return _wfd;
        yield return new WaitForSeconds(0.5f);

        BattleTest.Instance.StartBattle();
    }

    private void SetUpNewTurnEvents()
    {
        // Clear Previous Subscribers
        OnTurnPassed = null;

        // Make Stance Recovering
        OnTurnPassed += () =>
        {
            if (Player.CurrentStance > 0) Player.CurrentStance += Player.StanceRecover;

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                if (e.CurrentStance > 0) e.CurrentStance += e.StanceRecover;
            }

            _uiManager.UpdateStanceBars(_playerParty, _enemyParty);
            Debug.Log("DID STANCE RECOVER");
        };

        // Count Turns in Modifiers and Check Them
        OnTurnPassed += () =>
        {
            Player.CheckModifier();

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                e.CheckModifier();
            }
            Debug.Log("DID MODIFIERS COUNT");
        };

        // Check Bar Modifiers
        OnTurnPassed += _pullManager.CheckBarModifiers;

        // Count Turns in Moves
        OnTurnPassed += () =>
        {
            foreach (MoveInfo m in Player.MoveSet) m.TurnPassed();

            foreach (CharacterInfo c in _enemyParty.PartyMembers)
            {
                foreach (MoveInfo m in c.MoveSet) m.TurnPassed();
            }
            Debug.Log("DID MOVE COUNT");
        };

        // Make Enemies Choose Action Every Turn
        foreach (EnemyInfo enemy in _enemyParty.PartyMembers)
        {
            enemy.SetUpAI();
            OnTurnPassed += () =>
            {
                if (enemy.CurrentStance > 0)
                {
                    MoveInfo m = enemy.BattleAI.ChooseRandom();
                    AddAction(enemy, m);
                }
                Debug.Log("DID ENEMY AI");
            };
        }
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
            MoveInfo move = (i < Player.MoveSet.Count) ? Player.MoveSet?[i] : null;

            if (move != null)
            {
                moveButtons[i].gameObject.SetActive(true);
                moveButtons[i].onClick.RemoveAllListeners();

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
            MoveInfo move = (i < Player.MoveSet.Count) ? Player.MoveSet?[i] : null;

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
        _inventoryManager.ShowInventory(Player.Inventory);

        var invButtons = _inventoryManager.GetInventoryButtons();

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
                .SetUpHover(stack.Item, _inventoryManager);
            }
        }
    }

    // Battle Loop
    private IEnumerator BattleLoopCR()
    {
        while (true)
        {
            VerifyTurnLosses();

            CurrentTurn++;

            UpdateButtons();
            _uiManager.UpdateStatModifierDisplay(Player.StatModifiers);


            yield return new WaitUntil(() => _actionList.Count == _numberOfBattlers);

            if (_selectingBar)
            {
                _uiManager.ToggleSelecBar(true);
                _pullManager.ToggleBarButtons(true);

                yield return new WaitUntil(() => !_selectingBar);

                _uiManager.ToggleSelecBar(false);
                _pullManager.ToggleBarButtons(false);
            }

            _uiManager.ToggleMoveButtons(false);
            _uiManager.ToggleActionButtons(false);
            _uiManager.ToggleMoveInfo(false);
            _inventoryManager.HideInventory();

            OrganizeActions();

            foreach (BattleAction action in _actionList)
            {
                ExecuteAction(action);

                _uiManager.UpdateStanceBars(_playerParty, _enemyParty);
                _uiManager.UpdateStatModifierDisplay(Player.StatModifiers);
                _dialogueManager.StartDialogues();

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
            }

            // Clear Actions List
            _actionList.Clear();
            _uiManager.ToggleActionButtons(true);
        }
    }

    private void Win(bool playerWon)
    {
        Debug.Log($"WINNER IS PLAYER? {playerWon}");
        _playerWon = playerWon;
        _hasWinner = true;
    }

    private void ShowEnd()
    {
        Debug.Log("END BATTLE NOW");
        _uiManager.ToggleMoveButtons(false);
        _uiManager.ToggleActionButtons(false);
        _uiManager.ToggleMoveInfo(false);
        _dialogueManager.HideDialogue();
        _inventoryManager.HideInventory();

        _uiManager.ShowEndScreen(_playerWon);
    }
    
    private void VerifyTurnLosses()
    {
        // If stance = 0, character loses turn
        if (Player.CurrentStance == 0) AddAction(Player);

        foreach (CharacterInfo e in _enemyParty.PartyMembers)
        {
            if (e.CurrentStance == 0) AddAction(e);
        }
    }

    public void AddAction(CharacterInfo character)
    {
        var action = new BattleAction(character);
        _actionList.Add(action);
    }
    public void AddAction(CharacterInfo character, MoveInfo move)
    {
        var action = new BattleAction(character, move);
        _actionList.Add(action);

        if (character is PlayerInfo && move.Type == MoveTypes.Modifier) _selectingBar = true;
    }
    public void AddAction(CharacterInfo character, ItemInfo item)
    {
        var action = new BattleAction(character, item);
        _actionList.Add(action);
    }

    private void ExecuteAction(BattleAction action)
    {
        switch (action.Type)
        {
            case ActionType.Move:

                var party = action.Character is PlayerInfo ? _enemyParty : _playerParty;
                // var target = ChooseTarget(party);

                action.Move.UsedMove();

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Move.Name}.");
                _battleResolver.DoMove(action.Move, action.Character, party);
                break;

            case ActionType.Item:

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Item.Name}.");
                _battleResolver.UseItem(action.Item, action.Character);
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

        _actionList.Sort((a, b) => b.Character.Speed.CompareTo(a.Character.Speed));

        Debug.Log("ACTION ORDER");
        for (int i = 0; i < _actionList.Count; i++)
        {
            Debug.Log($"{_actionList[i].Character.Name}");
        }
    }

    private CharacterInfo ChooseTarget(PartyInfo fromParty)
    {
        int rnd = UnityEngine.Random.Range(0, fromParty.PartySize);
        CharacterInfo c = fromParty.PartyMembers[rnd];

        Debug.Log($"TARGETING {c.Name}");

        return c;
    }
}