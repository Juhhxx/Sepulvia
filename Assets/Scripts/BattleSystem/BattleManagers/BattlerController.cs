using UnityEngine;
using System.Collections.Generic;

public class BattlerController : MonoBehaviour
{
    private Character _character;
    public Character Character => _character;

    private StatusEffectManager _statusEffectManager;

    private void Awake()
    {
        _statusEffectManager = GetComponent<StatusEffectManager>();
    }

    public void SetUp(CharacterInfo characterInfo)
    {
        _character = new Character(characterInfo);
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
}
