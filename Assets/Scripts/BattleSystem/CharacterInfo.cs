using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Character")]
public class CharacterInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject BattlePrefab { get; private set; }

    [SerializeField] private int _baseStance;
    public int BaseStance => _baseStance;

    private int _currentStance;
    public int CurrentStance => _currentStance;

    [SerializeField] private int _baseStanceRecover;
    public int BaseStanceRecover => _baseStanceRecover;

    [field: SerializeField] public List<MoveInfo> MoveSet { get; private set; }
}
