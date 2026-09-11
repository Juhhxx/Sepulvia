using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Party", menuName = "Battle System/New Party")]
public class PartyInfo : DataAsset
{
    [field: SerializeField] public string PartyName { get; private set; }
    [SerializeField] private bool _isPlayerParty;

    // Player Party
    [field: SerializeField, ShowIf(nameof(_isPlayerParty))]
    public PlayerInfo Player { get; private set; }

    [field: SerializeField, HideIf(nameof(_isPlayerParty))]
    public List<CharacterInfo> PartyMembers { get; private set; }

    public int Difficulty => CalculatePartyDifficulty();

    private int CalculatePartyDifficulty()
    {
        int difficulty = 0;

        foreach (EnemyInfo e in PartyMembers) difficulty += e.DifficultyLevel;

        return difficulty;
    }

    public Party Instantiate()
    {
        return _isPlayerParty ? new PlayerParty(this) : new EnemyParty(this);
    }
}

[Serializable]
public class Party
{
    public Party(PartyInfo info)
    {
        PartyName = info.PartyName;
        
        _partyMembers = new List<Character>();

        foreach (CharacterInfo c in info.PartyMembers)
        {
            _partyMembers.Add(c.Instantiate());
        }
    }

    [field: SerializeField, ReadOnly] public string PartyName { get; private set; }
    [SerializeField, ReadOnly] public List<Character> _partyMembers;

    public IReadOnlyList<Character> PartyMembers => _partyMembers;
    public int PartySize => PartyMembers.Count;

    public event Action<Character> OnAddMember;
    public event Action<Character> OnRemoveMember;

    public void AddPartyMember(Character character)
    {
        _partyMembers.Add(character);
        OnAddMember?.Invoke(character);
    }
    public void RemovePartyMember(Character character)
    {
        _partyMembers.Remove(character);
        OnRemoveMember?.Invoke(character);
    }
}

public class EnemyParty : Party
{
    public EnemyParty(PartyInfo info) : base(info) {}
}

public class PlayerParty : Party
{
    private Player _player;
    public Player Player => _player;

    public PlayerParty(PartyInfo info) : base(info)
    {
        _player = info.Player.Instantiate() as Player;

        _partyMembers.Add(_player);
    }
}
