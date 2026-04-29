using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Party", menuName = "Battle System/New Party")]
public class PartyInfo : ScriptableObject
{
    [field: SerializeField] public string PartyName { get; private set; }
    [field: SerializeField, Expandable] public List<CharacterInfo> PartyMembers { get; private set; }
    public int PartySize => (PartyMembers?.Count).Value;

    public Party Instantiate()
    {
        return new Party(this);
    }
}

public class Party
{
    public Party(PartyInfo info)
    {
        PartyName = info.PartyName;
        PartyMembers = new List<Character>();

        foreach (CharacterInfo c in info.PartyMembers)
        {
            PartyMembers.Add(c.Instantiate());
        }
    }

    public string PartyName { get; private set; }
    public List<Character> PartyMembers { get; private set; }
    public int PartySize => PartyMembers.Count;
}
