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

    // Other Variables
    private WaitForDialogueEnd _wfd;

    private bool _hasWinner = false;
    private bool _playerWon = false;

    public event Action OnBattleEnd;
    public UnityEvent OnBattleWon;
    public UnityEvent OnBattleLost;


    // Run logic
    public void DoRun() => AddAction(Player, true);
    bool _doRun = false;
    public void Run()
    {
        bool result = _battleResolver.CanRun(Player, _enemyParty);

        _doRun = result;
    }
}