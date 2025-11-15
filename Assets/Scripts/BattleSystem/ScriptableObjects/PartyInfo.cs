using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Party", menuName = "Battle System/New Party")]
public class PartyInfo : ScriptableObject
{
    [field: SerializeField] public string PartyName { get; private set; }
    [field: SerializeField, Expandable] public List<CharacterInfo> PartyMembers { get; private set; }
    public int PartySize => (PartyMembers?.Count).Value;

    public PartyInfo Instantiate()
    {
        var p = Instantiate(this);

        for (int i = 0; i < p.PartySize; i++)
        {
            p.PartyMembers[i] = p.PartyMembers[i].Instantiate();
        }

        return p;
    }
}
