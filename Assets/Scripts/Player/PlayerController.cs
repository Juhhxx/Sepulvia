using NaughtyAttributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField, Expandable] public PartyInfo PlayerParty { get; private set; }
    public CharacterInfo PlayerCharacter => PlayerParty.PartyMembers[0];
    public bool InBattle { get; set; }

    private void Awake()
    {
        PlayerParty = PlayerParty.Instantiate();
    }
}
