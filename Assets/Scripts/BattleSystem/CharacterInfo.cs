using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Character")]
public class CharacterInfo : ScriptableObject
{
    [field: Header("Character Cosmetics")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject BattlePrefab { get; private set; }

    [Space(10)]
    [Header("Character Stats")]
    [Space(5)]
    [SerializeField] private int _baseSpeed;
    public int Speed => _baseSpeed;

    [SerializeField] private int _baseStance;
    public int Stance => _baseStance;

    private int _currentStance;
    public int CurrentStance => _currentStance;

    [SerializeField] private int _baseStanceRecover;
    public int StanceRecover => _baseStanceRecover;

    [field: Space(10)]
    [field: Header("Character Moves")]
    [field: Space(5)]
    [field: SerializeField, Expandable] public List<MoveInfo> MoveSet { get; private set; }

    [field: Space(10)]
    [field: Header("Character Cosmetics")]
    [field: Space(5)]
    [field: SerializeField] public bool IsPlayer { get; private set; }
}
