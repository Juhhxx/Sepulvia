using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Party", menuName = "Battle System/New Party")]
public class PartyInfo : ScriptableObject
{
    [field: SerializeField] public string PartyName { get; private set; }
    [field: SerializeField] public List<CharacterInfo> PartyMembers { get; private set; }
}
