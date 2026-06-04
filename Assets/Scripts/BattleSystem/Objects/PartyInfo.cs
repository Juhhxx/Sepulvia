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

    // Enemy Party
    [field: SerializeField, HideIf(nameof(_isPlayerParty))]
    public List<EnemyInfo> PartyMembers { get; private set; }

    public int PartySize => _isPlayerParty ? 1 : (PartyMembers?.Count).Value;

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
    }

    [field: SerializeField, ReadOnly] public string PartyName { get; private set; }
    public virtual int PartySize { get; private set; }
}

public class EnemyParty : Party
{
    [SerializeField, ReadOnly] public List<Enemy> _partyMembers;

    public IReadOnlyList<Enemy> PartyMembers => _partyMembers;
    public override int PartySize => PartyMembers.Count;


    public EnemyParty(PartyInfo info) : base(info)
    {
        _partyMembers = new List<Enemy>();

        foreach (EnemyInfo c in info.PartyMembers)
        {
            _partyMembers.Add(c.Instantiate() as Enemy);
        }
    }
}

public class PlayerParty : Party
{
    private Player _player;
    public Player Player => _player;
    public override int PartySize => 1;

    public PlayerParty(PartyInfo info) : base(info)
    {
        _player = info.Player.Instantiate() as Player;
    }
}
