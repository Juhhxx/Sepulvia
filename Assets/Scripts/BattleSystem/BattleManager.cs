using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private BattleUIManager _uiManager;

    [Space(10)]
    [Header("Battlers Info")]
    [Space(5)]
    [SerializeField, Expandable] private PartyInfo _playerParty;
    [SerializeField, Expandable] private PartyInfo _enemyParty;
    
    private int _numberOfBattlers;

    // Battle Action Variables
    [SerializeField, ReadOnly] private List<Action> _actionList;

    public enum ActionType { Move, Item, Empty }

    [Serializable]
    public struct Action
    {
        public CharacterInfo Character { get; private set; }
        public ActionType Type { get; private set;}
        public MoveInfo Move { get; private set; }
        public ItemInfo Item { get; private set;}

        public Action(CharacterInfo character, ActionType type, MoveInfo move)
        {
            Character = character;
            Type = type;
            Move = move;
            Item = null;
        }

        public Action(CharacterInfo character, ActionType type, ItemInfo item)
        {
            Character = character;
            Type = type;
            Move = null;
            Item = item;
        }

        public Action(CharacterInfo character, ActionType type)
        {
            Character = character;
            Type = type;
            Move = null;
            Item = null;
        }
    }

    // Battle Turn Variables
    public event System.Action OnTurnPassed;
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

    public void StartBattle(PartyInfo playerParty, PartyInfo enemyParty)
    {
        _playerParty = playerParty;
        _enemyParty = enemyParty;

        _numberOfBattlers = playerParty.PartySize + enemyParty.PartySize;

        StopAllCoroutines();
        StartCoroutine(BattleLoopCR());
    }

    private IEnumerator BattleLoopCR()
    {
        yield return new WaitUntil(() => _actionList.Count == _numberOfBattlers);

        OrganizeActions();

        foreach (Action action in _actionList)
        {
            ExecuteAction(action);
        }

        _actionList.Clear();

        CurrentTurn++;
    }

    private void ExecuteAction(Action action)
    {
        switch (action.Type)
        {
            case ActionType.Move:

                var party = action.Character.IsPlayer ? _enemyParty : _playerParty;
                var target = ChooseTarget(party);

                _battleResolver.DoMove(action.Move, action.Character, target);
                break;

            case ActionType.Item:

                _battleResolver.UseItem(action.Item, action.Character);
                break;
                
            case ActionType.Empty:

                break;
        }
    }

    private CharacterInfo ChooseTarget(PartyInfo fromParty)
    {
        int rnd = UnityEngine.Random.Range(0, fromParty.PartySize);
        CharacterInfo c = fromParty.PartyMembers[rnd];

        Debug.Log($"TARGETING {c.Name}");

        return c;
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
}
