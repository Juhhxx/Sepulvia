using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Enemy Character")]
public class EnemyInfo : CharacterInfo
{
    [field: Header("Character Overworld Cosmetics")]
    [field: Space(5)]
    [field : SerializeField] public GameObject OverworldPrefab { get; private set; }

    [field: Header("Character Phases")]
    [field: Space(5)]
    [field : SerializeField] public PartyInfo NextPhase;

    [field: Header("Character Defeat Rewards")]
    [field: Space(5)]
    [field : SerializeField] public bool CanRun{ get; private set; }
    [field : SerializeField] public int DifficultyLevel { get; private set; }
    [field : SerializeField] public List<ItemInfo> PossibleRewards { get; private set; }

    [field: Space(10)]
    [field: Header("Character Battle AI")]
    [field: Space(5)]
    [field : SerializeField] public int Inteligence { get; private set; }
}

public class Enemy : Character
{
    public Enemy(EnemyInfo info) : base(info)
    {
        NextPhase = info.NextPhase?.Instantiate();
        
        if (NextPhase != null)
        {
            foreach (Character c in NextPhase.PartyMembers)
            {
                (c as Enemy).SetPhase(Phase + 1);
            }
        }
        
        DifficultyLevel = info.DifficultyLevel;
        PossibleRewards = info.PossibleRewards;
        Inteligence = info.Inteligence;
        CanRun = info.CanRun;
    }

    [field: Header("Character Phases")]
    [field: Space(5)]
    [field : SerializeField] public Party NextPhase;
    public int Phase { get; private set; } = 1;
    public void SetPhase(int phase) => Phase = phase;

    [field: Header("Character Defeat Rewards")]
    [field: Space(5)]
    [field : SerializeField, ReadOnly] public int DifficultyLevel { get; private set; }
    [field : SerializeField, ReadOnly] public bool CanRun{ get; private set; }
    [field : SerializeField, ReadOnly] public List<ItemInfo> PossibleRewards { get; private set; }

    [field: Space(10)]
    [field: Header("Character Battle AI")]
    [field: Space(5)]
    [field : SerializeField, ReadOnly] public int Inteligence { get; private set; }

}
