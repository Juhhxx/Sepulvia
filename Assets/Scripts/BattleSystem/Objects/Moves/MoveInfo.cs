using NaughtyAttributes;
using UnityEngine;
using System;
using TNRD;

[CreateAssetMenu(fileName = "Move", menuName = "Battle System/New Move")]
public class MoveInfo : ScriptableObject
{
    [field: Header("Base Move Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [field: Header("Move Cost Parameters")]
    [field: Space(5)]
    [field: SerializeField] public int RecoveryCost { get; private set; }
    [field: SerializeField] public int Cooldown { get; private set; }

    [Space(10)]
    [Header("Stance Move Parameters")]
    [Space(5)]

    [ShowIf("Type", MoveTypes.Stance)]
    [SerializeField] private int _stanceCost;
    public int StanceCost => _stanceCost;

    [field: Space(10)]
    [field: Header("Move Type Parameters")]
    [field: Space(5)]
    [field: SerializeField] public MoveTypes Type { get; private set; }
    [field: SerializeField] public MoveTargeting Targeting { get; private set; }

    [SerializeField] private SerializableInterface<IMove> _moveLogic;
    public SerializableInterface<IMove> MoveLogic => _moveLogic;

    public Move Instantiate()
    {
        return new Move(this);
    }
}

[Serializable]
public class Move
{
    public Move(MoveInfo info)
    {
        Name = info.Name;
        Icon = info.Icon;
        Description = info.Description;
        Level = info.Level;

        Cooldown = info.Cooldown;
        RecoveryCost = info.RecoveryCost;

        Type = info.Type;
        Targeting = info.Targeting;

        StanceCost = info.StanceCost;

        _moveLogic = info.MoveLogic;
    }

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; }
    [field: SerializeField, ResizableTextArea, ReadOnly] public string Description { get; private set; }
    [field: SerializeField, ReadOnly] public int Level { get; private set; }

    [field: SerializeField] public int RecoveryCost { get; private set; }

    [field: SerializeField, ReadOnly] public int Cooldown { get; private set; }
    [SerializeField, ReadOnly] private int _turnsPassed = 0;
    [SerializeField, ReadOnly] private bool _inCooldown = false;
    public void TurnPassed()
    {
        if (_inCooldown) _turnsPassed++;
        if (_turnsPassed == Cooldown + 1) // Don't cout first turn
        {
            _inCooldown = false;
            _turnsPassed = 0;
        }
    }
    public void UsedMove() => _inCooldown = true;
    public bool CheckIfCooldown() => _inCooldown;
    public void ResetCooldown()
    {
        _inCooldown = false;
        _turnsPassed = 0;
    }

    [field: SerializeField, ReadOnly] public MoveTypes Type { get; private set; }
    [field: SerializeField] public MoveTargeting Targeting { get; private set; }

    [field: SerializeField, ReadOnly] public int StanceCost { get; private set; }
    public bool CheckIfStanceCost(Character user) => user.CurrentStance >= StanceCost;

    public bool CheckIfCanUseMove(Character user)
    {
        if (CheckIfCooldown()) return false;
        if (Type == MoveTypes.Stance)
            if (!CheckIfStanceCost(user)) return false;

        return true;
    }

    [SerializeField] private SerializableInterface<IMove> _moveLogic;
    public IMove MoveLogic => _moveLogic.Value;
}

public interface IMove
{
    public void OnDoMove(Character user, Character[] targets);
}
