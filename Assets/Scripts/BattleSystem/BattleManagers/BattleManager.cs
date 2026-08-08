using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;
    [SerializeField] private TimelineManager _timelineManager;
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

    // Battle Actions
    [SerializeField, ReadOnly] private List<BattleAction> _actionList = new List<BattleAction>();

    public void AddAction(Character character)
    {
        var action = new BattleAction(character);
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
        }
    }

    private void OrganizeBattlers()
    {
        _battlersList = _enemyParty.PartyMembers.Concat(new List<Character> { Player }).ToList();

        _battlersList = _battlersList
                        .OrderByDescending(b => b.Speed)
                        .ToList();
        
        Debug.Log("BATTLER ORDER:");
        for (int i = 0; i < _battlersList.Count; i++)
        {
            Debug.Log($"{_battlersList[i].Name}, Speed: {_battlersList[i].Speed}");
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


    public event Action OnBattleEnd;
    public UnityEvent OnBattleWon;
    public UnityEvent OnBattleLost;


    // Run logic
    public void DoRun() => AddAction(Player);
    public void Run()
    {
        bool result = _battleResolver.CanRun(Player, _enemyParty);

        if (result)
        {
            EndBattle();
        }
    }

    // Battle logic
    public void StartBattle(PlayerParty playerParty, EnemyParty enemyParty)
    {
        _playerParty = playerParty;
        _enemyParty = enemyParty;


    }
    private void EndBattle()
    {
        
    }

    private IEnumerator ResolveTurn()
    {
        OrganizeBattlers();

        foreach (var battler in _battlersList)
        {
            if (battler.RecoveryTime <= 0)
            {
                BattleAction action = null;

                
                if (!battler.HasActions())
                {
                    if (battler is Player)
                    {
                        _timelineManager.ToggleTurnCounting(false);

                        // Show ui to player
                    }
                    else
                    {
                        var tmp = (battler as Enemy).BattleAI.ChooseRandom(_pullManager.BarSections, _pullManager.CurrentHeartIndex);

                        battler.QueueAction(tmp);
                    }
                }

                action = battler.GetAction();

                yield return new WaitUntil(() => action != null);

                _timelineManager.ToggleTurnCounting(true);

                // Hide ui from player

                ExecuteAction(action);
            }
        }
    }
}