using UnityEngine;

public class EnemyPartyDataBase : DataBase<PartyInfo>
{
    public PartyInfo GetRandomParty(System.Random random, int playerLevel)
    {
        var filteredEntries = _entries.FindAll(p => p.Difficulty <= playerLevel);

        int rnd = random.Next(0, filteredEntries.Count);

        return filteredEntries[rnd];
    }
}
