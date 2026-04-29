using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "Move", menuName = "Battle System/New Move")]
public class MoveInfo : ScriptableObject
{
    [field: Header("Base Move Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public MoveTypes Type { get; private set; }

    [field: SerializeField] public int PriorityLevel { get; private set; }

    [field: SerializeField] public int StanceCost { get; private set; }

    [field: SerializeField] public int StanceDamage { get; private set; }

    [field: SerializeField] public int Cooldown { get; private set; }

    [Space(10)]
    [Header("Pull Move Parameters")]
    [Space(5)]
    [ShowIf("Type", MoveTypes.Pull)]
    [SerializeField] private int _pullStrength;
    public int PullStrength => _pullStrength;

    private bool IsEffect => Type == MoveTypes.Buff || Type == MoveTypes.Nerf;

    [field: Space(10)]
    [field: Header("Buff/Nerf Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("IsEffect")]
    [field: SerializeField] public List<StatModifier> StatModifiers { get; private set; }

    [field: Space(10)]
    [field: Header("Modifier Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", MoveTypes.Modifier)]
    [field: SerializeField] public BarModifier Modifier { get; private set; }

    [field: Space(10)]
    [field: Header("Move Description")]
    [field: Space(5)]
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }

    public Move Instantiate()
    {
        return new Move(this);
    }
}

public class Move
{
    public Move(MoveInfo info)
    {
        Name = info.Name;

        Type = info.Type;
        PriorityLevel = info.PriorityLevel;

        StanceCost = info.StanceCost;
        StanceDamage = info.StanceDamage;

        Cooldown = info.Cooldown;

        _pullStrength = info.PullStrength;

        StatModifiers = new List<StatModifier>(info.StatModifiers);
        Modifier = info.Modifier;
        
        Description = info.Description;
    }

    [field: Header("Base Move Parameters")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public string Name { get; private set; }

    [field: SerializeField, ReadOnly] public MoveTypes Type { get; private set; }

    [field: SerializeField, ReadOnly] public int PriorityLevel { get; private set; }

    [field: SerializeField, ReadOnly] public int StanceCost { get; private set; }
    public bool CheckStanceCost(Character character) => character.CurrentStance >= StanceCost;

    [field: SerializeField, ReadOnly] public int StanceDamage { get; private set; }

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

    [Space(10)]
    [Header("Pull Move Parameters")]
    [Space(5)]
    [ShowIf("Type", MoveTypes.Pull)]
    [SerializeField, ReadOnly] private int _pullStrength;
    public int PullStrength => _pullStrength;

    private bool IsEffect => Type == MoveTypes.Buff || Type == MoveTypes.Nerf;

    [field: Space(10)]
    [field: Header("Buff/Nerf Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("IsEffect")]
    [field: SerializeField, ReadOnly] public List<StatModifier> StatModifiers { get; private set; }

    [field: Space(10)]
    [field: Header("Modifier Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", MoveTypes.Modifier)]
    [field: SerializeField, ReadOnly] public BarModifier Modifier { get; private set; }

    [field: Space(10)]
    [field: Header("Move Description")]
    [field: Space(5)]
    [field: SerializeField, ResizableTextArea, ReadOnly] public string Description { get; private set; }
}
