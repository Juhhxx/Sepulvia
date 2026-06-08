using UnityEngine;

public class EnemyPartyDataBase : DataBase<PartyInfo>
{
    public PartyInfo GetRandomParty(System.Random random)
    {
        int rnd = random.Next(0, _entries.Count);
        return _entries[rnd];
    }
}
