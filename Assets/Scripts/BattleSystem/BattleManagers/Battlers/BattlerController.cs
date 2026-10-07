using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using System;
using System.Linq;

public class BattlerController : MonoBehaviour
{
    // Character
    [SerializeField] private Character _character;
    public Character Character => _character;
    public bool IsPlayer() => _character is Player;

    // Animation

    private BattlerAnimationController _animationController;
    public BattlerAnimationController AnimationController => _animationController;

    // Battle Parameters
    private bool _isStunned = false;
    public void SetStunned(bool stunned)
    {
        _isStunned = stunned;
    }
    public bool IsBlocking => _statusEffectManager.HasStatusEffect<StatusEffectBlock>();
    public event Action OnDoBlock;
    public void OnBlock() => OnDoBlock?.Invoke();
    public bool IsInterruped => _statusEffectManager.HasStatusEffect<StatusEffectInterrupt>();

    [SerializeField] private int _battleItemCooldown;
    private int _itemsCooldownTurns;
    public int ItemCooldown => _battleItemCooldown;
    public int ItemColldownTurns => _itemsCooldownTurns;
    public void CountItemTurn()
    {
        if (_itemsCooldownTurns == 0) return;
        _itemsCooldownTurns -= 1;
    }

    private bool _canUseItems = true;
    public bool CanUseItems => _canUseItems;

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
        _animationController = GetComponent<BattlerAnimationController>();

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

    // Creating Actions
    public void RequestAction()
    {
        StartCoroutine(CreateAction());
    }
    private IEnumerator CreateAction()
    {
        yield return new WaitUntil(() => _actionMove != null || _actionItemStack != null);

        var possibleTargets = _battleManager.GetBattlerControllers(_battleManager.EnemyParty.PartyMembers.ToArray());

        if (_actionMove != null)
        {
            var targeting = _actionMove.Targeting;

            if (targeting == MoveTargeting.Single && possibleTargets.Length == 1)
            {
                targeting = MoveTargeting.All;
            }

            switch (targeting)
            {
                case MoveTargeting.Single:

                    _targetButtonManager.RequestTargets(1, this, possibleTargets);

                    yield return new WaitUntil(() => _actionTargets.Count == 1);

                    AddAction(_actionMove, _actionTargets.ToArray());
                    break;
                
                case MoveTargeting.Self:

                    _actionTargets.Add(this);
                    break;
                
                case MoveTargeting.All:

                    _actionTargets.AddRange(possibleTargets);
                    break;

            }

            AddAction(_actionMove, _actionTargets.ToArray());
        }
        else if(_actionItemStack != null)
        {
            AddAction(_actionItemStack, possibleTargets);
        }

        _actionMove = null;
        _actionItemStack = null;
        _actionTargets.Clear();
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

    private ItemStack _actionItemStack = null;

    public void SetItem(ItemStack stack) => _actionItemStack = stack;
    public void AddAction(ItemStack stack, BattlerController[] targets)
    {
        bool canUseItems = _itemsCooldownTurns == 0;

        if (IsPlayer() && !canUseItems) return;

        var action = new BattleAction(this, targets, stack);

        _battleManager.ExecuteAction(action);

        if (IsPlayer())
        {
            _itemsCooldownTurns = _battleItemCooldown;
        }
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