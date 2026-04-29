using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Enemy Character")]
public class EnemyInfo : CharacterInfo
{
    [field: Header("Character Defeat Rewards")]
    [field: Space(5)]
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
        DifficultyLevel = info.DifficultyLevel;
        PossibleRewards = info.PossibleRewards;
        Inteligence = info.Inteligence;
        BattleAI = new EnemyBattleAI(this);
    }

    [field: Header("Character Defeat Rewards")]
    [field: Space(5)]
    [field : SerializeField, ReadOnly] public int DifficultyLevel { get; private set; }
    [field : SerializeField, ReadOnly] public List<ItemInfo> PossibleRewards { get; private set; }

    [field: Space(10)]
    [field: Header("Character Battle AI")]
    [field: Space(5)]
    [field : SerializeField, ReadOnly] public int Inteligence { get; private set; }

    public EnemyBattleAI BattleAI { get; private set; }
}
