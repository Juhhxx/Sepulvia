using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;

public class BattlerController : MonoBehaviour
{
    // Character
    [SerializeField] private Character _character;
    public Character Character => _character;
    public bool IsPlayer() => _character is Player;

    // Battle Parameters
    private bool _isStunned = false;
    public void SetStunned(bool stunned)
    {
        _isStunned = stunned;
    }
    public bool IsBlocking => _statusEffectManager.HasStatusEffect<StatusEffectBlock>();
    public bool IsInterrupting => _statusEffectManager.HasStatusEffect<StatusEffectInterrupt>();

    // Enemy Battle AI
    [SerializeField] private EnemyBattleAI _enemyBattleAI;
    public EnemyBattleAI BattleAI => _enemyBattleAI;

    // Status Effect Manager
    [SerializeField] private StatusEffectManager _statusEffectManager;
    public StatusEffectManager StatusEffectManager => _statusEffectManager;

    // Set Up
    private BattleManager _battleManager;
    private TargetButtonManager _targetButtonManager;

    public void SetUp(Character character, BattleManager battleManager)
    {
        _character = character;
        _battleManager = battleManager;

        _enemyBattleAI = GetComponent<EnemyBattleAI>();

        if (IsPlayer())
        {
            _targetButtonManager = FindAnyObjectByType<TargetButtonManager>(FindObjectsInactive.Include);
        }
    }

    // Actions
    private Queue<BattleAction> _queuedActions = new Queue<BattleAction>();
    public Queue<BattleAction> QueuedActions => _queuedActions;

    private List<BattleActionRegister> _actionTimeline = new List<BattleActionRegister>();
    public IReadOnlyList<BattleActionRegister> ActionTimeline => _actionTimeline;

    public void QueueAction(BattleAction action)
    {
        _queuedActions.Enqueue(action);
    }

    public BattleAction GetAction(int turn)
    {
        var action = _queuedActions.Dequeue();
        
        _actionTimeline.Add(new BattleActionRegister(turn, action));

        return action;
    }

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
                    _actionTargets.Add(this);
                }
                else
                {
                    foreach (Character c in _battleManager.EnemyParty.PartyMembers)
                    {
                        _actionTargets.Add(_battleManager.GetBattlerController(c));
                    }
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
    private List<BattlerController> _actionTargets = new List<BattlerController>();

    public void SetMove(Move move)
    {
        _actionMove = move;
        _battleManager.CurrentState = BattleManager.BattleState.PlayerChooseTarget;
    }
    public void AddTarget(BattlerController target)
    {
        _actionTargets.Add(target);
    }
    public void AddAction(Move move, BattlerController[] targets)
    {
        var action = new BattleAction(this, targets, move);

        QueueAction(action);
    }

    private ItemInfo _actionItem = null;

    public void SetItem(ItemInfo item) => _actionItem = item;
    public void AddAction(ItemInfo item)
    {
        var action = new BattleAction(this, item);

        QueueAction(action);
    }

    public void AddActionRun()
    {
        var action = new BattleAction(this);

        QueueAction(action);
    }

}

public class BattleActionRegister
{
    public int Turn { get; private set; }
    public BattleAction Action { get; private set; }

    public BattleActionRegister(int turn, BattleAction action)
    {
        Turn = turn;
        Action = action;
    }
}