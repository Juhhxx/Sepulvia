using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

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

    private void Start()
    {
#if UNITY_EDITOR
        if (_playerParty != null && _enemyParty != null) StartBattle(_playerParty, _enemyParty);
#endif
    }

    public void StartBattle(PartyInfo playerParty, PartyInfo enemyParty)
    {
        _playerParty = playerParty.Instantiate();
        _enemyParty = enemyParty.Instantiate();

        _uiManager.InstantiateBattlePrefabs(_playerParty, _enemyParty);
        _dialogueManager.SetUpDialogueManager();
        _pullManager.OnSelectBar += () => _selectingBar = false;

        _wfd = new WaitForDialogueEnd(_dialogueManager);

        _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        SetUpNewTurnEvents();

        SetUpBattleUI();

        StopAllCoroutines();
        StartCoroutine(BattleLoopCR());
    }

    private void SetUpNewTurnEvents()
    {
        // Make Stance Recovering
        OnTurnPassed += () =>
        {
            Player.CurrentStance += Player.StanceRecover;

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                e.CurrentStance += e.StanceRecover;
            }

            _uiManager.UpdateStanceBars(_playerParty, _enemyParty);
        };

        // Count Turns in Moves and Check Them
        OnTurnPassed += () =>
        {
            Player.CheckModifier();

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                e.CheckModifier();
            }
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
        };

        // Make Enemies Choose Action Every Turn
        foreach (EnemyInfo enemy in _enemyParty.PartyMembers)
        {
            enemy.SetUpAI();
            OnTurnPassed += () =>
            {
                MoveInfo m = enemy.BattleAI.ChooseRandom();
                AddAction(enemy, m);
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

            if (stack != null && stack.Item.CanBeUsedInBattle)
            {
                invButtons[i].enabled = true;
                invButtons[i].onClick.AddListener(() =>
                {
                    AddAction(Player, stack.Item);
                    Player.Inventory.RemoveItem(stack);
                });

                invButtons[i]?.GetComponent<ItemHoverInfo>().SetUpHover(stack.Item, _inventoryManager);
            }
            else invButtons[i].enabled = false;
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
            }

            // Clear Actions List
            _actionList.Clear();
            _uiManager.ToggleActionButtons(true);
        }
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
                var target = ChooseTarget(party);

                action.Move.UsedMove();

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Move.Name}.");
                _battleResolver.DoMove(action.Move, action.Character, target);
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