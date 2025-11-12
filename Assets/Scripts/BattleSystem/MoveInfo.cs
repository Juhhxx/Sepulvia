using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Move", menuName = "Battle System/New Move")]
public class MoveInfo : ScriptableObject
{
    [field: Header("Base Move Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public MoveTypes Type { get; private set; }
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
    [field: SerializeField] public List<StatModifier> Stats { get; private set; }

    [field: Space(10)]
    [field: Header("Move Description")]
    [field: Space(5)]
    [field: SerializeField, TextArea] public string Description;
}
