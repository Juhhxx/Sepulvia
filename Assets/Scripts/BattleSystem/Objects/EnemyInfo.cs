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
    private EnemyBattleAI _battleAI;
    public EnemyBattleAI BattleAI => _battleAI;

    public void SetUpAI()
    {
        _battleAI = new EnemyBattleAI(this);
    }

}
