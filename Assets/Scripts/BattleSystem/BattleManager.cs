using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;

public class BattleManager : MonoBehaviourSingleton<BattleManager>
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;
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
        public CharacterInfo Character { get; private set; }
        public ActionType Type { get; private set;}
        public MoveInfo Move { get; private set; }
        public ItemInfo Item { get; private set;}

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

        _wfd = new WaitForDialogueEnd(_dialogueManager);

        _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        // Make Enemies Choose Action Every Turn
        foreach (EnemyInfo enemy in _enemyParty.PartyMembers)
        {
            enemy.SetUpAI();
            OnTurnPassed += () =>
            {
                int rnd = enemy.BattleAI.ChooseRandom(enemy.MoveSet.Count);
                MoveInfo m = enemy.MoveSet[rnd];
                AddAction(enemy, m);
            };
        }

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

        // Make Modifier Checking
        OnTurnPassed += () =>
        {
            Player.CheckModifier();

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                e.CheckModifier();
            }
        };

        // Count Turns in Moves
        OnTurnPassed += () =>
        {
            foreach (MoveInfo m in Player.MoveSet) m.TurnPassed();

            foreach (CharacterInfo c in _enemyParty.PartyMembers)
            {
                foreach (MoveInfo m in c.MoveSet) m.TurnPassed();
            }
        };

        SetUpBattleUI();

        StopAllCoroutines();
        StartCoroutine(BattleLoopCR());
    }

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
                _uiManager.SetUpButton(moveButtons[i], move.Name,
                !move.CheckIfCooldown() && move.CheckStanceCost(Player));

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
                _uiManager.SetUpButton(moveButtons[i], move.Name,
                !move.CheckIfCooldown() && move.CheckStanceCost(Player));

                Debug.Log($"UPDATE MOVE BUTON FOR {move.Name}");
            }
        }
    }

    private IEnumerator BattleLoopCR()
    {
        while (true)
        {
            // If stance = 0, character loses turn
            if (Player.CurrentStance == 0) AddAction(Player);

            foreach (CharacterInfo e in _enemyParty.PartyMembers)
            {
                if (e.CurrentStance == 0) AddAction(e);
            }

            CurrentTurn++;

            UpdateButtons();

            yield return new WaitUntil(() => _actionList.Count == _numberOfBattlers);

            _uiManager.ToogleMoveButtons(false);
            _uiManager.ToogleActionButtons(false);

            OrganizeActions();

            foreach (BattleAction action in _actionList)
            {
                ExecuteAction(action);

                _uiManager.UpdateStanceBars(_playerParty, _enemyParty);
                _dialogueManager.StartDialogues();

                yield return _wfd;
                yield return new WaitForSeconds(0.5f);
            }

            // Clear Actions List
            _actionList.Clear();
            _uiManager.ToogleActionButtons(true);
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

                _battleResolver.DoMove(action.Move, action.Character, target);

                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Move.Name}.");
                break;

            case ActionType.Item:

                _battleResolver.UseItem(action.Item, action.Character);
                _dialogueManager.AddDialogue($"{action.Character.Name} used {action.Item.Name}.");
                break;

            case ActionType.Empty:

                action.Character.CurrentStance = action.Character.MaxStance;
                _dialogueManager.AddDialogue($"{action.Character.Name} lost their stance and took a turn to recover their balance.");
                break;
        }
    }
    private void OrganizeActions()
    {
        for (int i = 0; i < _actionList.Count - 1; i++)
        {
            if (_actionList[i].Character.Speed < _actionList[i + i].Character.Speed)
            {
                var tmp = _actionList[i];

                _actionList[i] = _actionList[i + 1];
                _actionList[i + 1] = tmp;
            }
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
