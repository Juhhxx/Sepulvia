using UnityEngine;
using System.Collections.Generic;

public class BattlerController : MonoBehaviour
{
    // Character
    private Character _character;
    public Character Character => _character;
    public bool IsPlayer() => _character is Player;

    // Set Up
    private void Awake()
    {
        _statusEffectManager = GetComponent<StatusEffectManager>();
    }

    public void SetUp(CharacterInfo characterInfo)
    {
        _character = new Character(characterInfo);
    }

    // Status Effects
    private StatusEffectManager _statusEffectManager;
    public StatusEffectManager StatusEffectManager => _statusEffectManager;

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
}
