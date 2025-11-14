using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Enemy Character")]
public class EnemyInfo : CharacterInfo
{
    [field: Header("Character Battle AI")]
    [field: Space(5)]
    [field : SerializeField] public int Inteligence { get; private set; }
    private EnemyBattleAI _battleAI;
    public EnemyBattleAI BattleAI => _battleAI;

    public void SetUpAI()
    {
        _battleAI = new EnemyBattleAI();
    }

}
