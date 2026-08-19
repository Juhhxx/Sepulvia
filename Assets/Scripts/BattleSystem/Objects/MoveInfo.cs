using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Move", menuName = "Battle System/New Move")]
public class MoveInfo : MoveBaseInfo
{
    [field: Space(10)]
    [field: Header("Move Type Parameters")]
    [field: Space(5)]
    [field: SerializeField] public MoveTypes Type { get; private set; }

    [Space(10)]
    [Header("Pull Move Parameters")]
    [Space(5)]
    [ShowIf("Type", MoveTypes.Pull)]
    [SerializeField] private int _pullStrength;
    public int PullStrength => _pullStrength;

    [field: Space(10)]
    [field: Header("Buff/Nerf Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("IsEffect")]
    [field: SerializeField] public List<StatModifier> StatModifiers { get; private set; }

    private bool IsEffect => Type == MoveTypes.Buff || Type == MoveTypes.Nerf;

    [Space(10)]
    [Header("Block Move Parameters")]
    [Space(5)]
    [ShowIf("Type", MoveTypes.Block)]
    [SerializeField] private int _stunTime;
    public int StunTime => _stunTime;

    public Move Instantiate()
    {
        return new Move(this);
    }
}

[Serializable]
public class Move : MoveBase
{
    public Move(MoveInfo info): base(info)
    {
        Type = info.Type;

        _pullStrength = info.PullStrength;
        StatModifiers = new List<StatModifier>(info.StatModifiers);
    }

    [field: SerializeField, ReadOnly] public MoveTypes Type { get; private set; }

    [ShowIf("Type", MoveTypes.Pull)]
    [SerializeField, ReadOnly] private int _pullStrength;
    public int PullStrength => _pullStrength;

    private bool IsEffect => Type == MoveTypes.Buff || Type == MoveTypes.Nerf;

    [field: Space(10)]
    [field: Header("Buff/Nerf Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("IsEffect")]
    [field: SerializeField, ReadOnly] public List<StatModifier> StatModifiers { get; private set; }
    
}
