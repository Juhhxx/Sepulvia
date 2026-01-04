using NaughtyAttributes;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField, Expandable] public PartyInfo PlayerParty { get; private set; }
    public CharacterInfo PlayerCharacter => PlayerParty.PartyMembers[0];

    private bool _inBattle = false;
    public bool InBattle 
    {
        get => _inBattle;

        set
        {
            if (value != _inBattle)
            {
                OnBattleEnterExit?.Invoke(value);
            }

            _inBattle = value;
        }
    }
    public event Action<bool> OnBattleEnterExit;

    private void Awake()
    {
        PlayerParty = PlayerParty.Instantiate();
    }
}
