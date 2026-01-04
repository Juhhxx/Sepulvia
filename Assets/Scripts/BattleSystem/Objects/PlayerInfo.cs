using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Player Character")]
public class PlayerInfo : CharacterInfo
{
    [field: SerializeField] public int Essence { get; set; }
}
