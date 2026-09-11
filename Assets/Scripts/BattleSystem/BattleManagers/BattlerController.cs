using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattlerController : MonoBehaviour
{
    // Character
    private Character _character;
    public Character Character => _character;
    public bool IsPlayer() => _character is Player;

    // Set Up
    private BattleManager _battleManager;
    private TargetButtonManager _targetButtonManager;

    public void SetUp(Character character, BattleManager battleManager)
    {
        _character = character;
        _battleManager = battleManager;

        _targetButtonManager = FindAnyObjectByType<TargetButtonManager>(FindObjectsInactive.Include);
    }

    // Actions
    private Queue<BattleAction> _queuedActions = new Queue<BattleAction>();

    public void QueueAction(BattleAction action)
    {
        _queuedActions.Enqueue(action);
    }

    public BattleAction GetAction() => _queuedActions.Dequeue();

    public bool HasActions() => _queuedActions.Count > 0;

    public void ClearActions()
    {
        _queuedActions.Clear();
    }

    // Creating Action
    public void RequestAction()
    {
        StartCoroutine(CreateAction());
    }
    private IEnumerator CreateAction()
    {
        yield return new WaitUntil(() => _actionMove != null || _actionItem != null);

        if (_actionMove != null)
        {
            if (_actionMove.Targeting == MoveTargeting.Single)
            {
                _targetButtonManager.RequestTargets(1);

                yield return new WaitUntil(() => _actionTargets.Count == 1);

                AddAction(_actionMove, _actionTargets.ToArray());
            }
            else
            {
                if (_actionMove.Targeting == MoveTargeting.Self)
                {
                    _actionTargets.Add(_character);
                }
                else
                {
                    _actionTargets.AddRange(_battleManager.EnemyParty.PartyMembers);
                }

                AddAction(_actionMove, _actionTargets.ToArray());
            }
        }
        else if(_actionItem != null)
        {
            AddAction(_actionItem);
        }

        _actionMove = null;
        _actionTargets.Clear();
        _actionItem = null;
    }

    private Move _actionMove = null;
    private List<Character> _actionTargets = new List<Character>();

    public void SetMove(Move move)
    {
        _actionMove = move;
        _battleManager.CurrentState = BattleManager.BattleState.PlayerChooseTarget;
    }
    public void AddTarget(Character character)
    {
        _actionTargets.Add(character);
    }
    public void AddAction(Move move, Character[] targets)
    {
        var action = new BattleAction(_character, targets, move);

        QueueAction(action);
    }

    private ItemInfo _actionItem = null;

    public void SetItem(ItemInfo item) => _actionItem = item;
    public void AddAction(ItemInfo item)
    {
        var action = new BattleAction(_character, item);

        QueueAction(action);
    }

    public void AddActionRun()
    {
        var action = new BattleAction(_character);

        QueueAction(action);
    }

}
