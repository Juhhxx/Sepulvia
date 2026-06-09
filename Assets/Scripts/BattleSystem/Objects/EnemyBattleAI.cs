using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyBattleAI
{
    private Character _character;

    public EnemyBattleAI(Character character)
    {
        _character = character;
    }
    
    public Move ChooseRandom(List<BarSection> sections, int soulPosition)
    {
        Random rnd = new Random();
        Move move = null;

        bool ok = false;
        int maxIteration = 5;
        int iteration = 0;

        while (!ok && iteration < maxIteration)
        {
            move = _character.MoveSet[rnd.Next(_character.MoveSet.Count)];

            ok = !move.CheckIfCooldown() && move.CheckStanceCost(_character);

            iteration++;
        }

        if (move == null)
        {
            UnityEngine.Debug.Log("MAX ITERATION REACHED, CHOOSING FIRST AVAILABLE MOVE");
            move = _character.MoveSet[0];
        }

        if (move.Type == MoveTypes.Modifier)
        {
            var occupiedBars = sections.FindAll(s => s.HasModifier).Select(s => sections.IndexOf(s));

            if (move.Modifier.Type == BarModifierType.GravityPull && occupiedBars.Contains(0)) return ChooseRandom(sections, soulPosition);

            move.SetBarSection(ChooseBarSection(rnd, move.Modifier, sections.Count, soulPosition, occupiedBars.ToArray()));
        }

        return move;
    }

    public int ChooseBarSection(Random rnd, BarModifier modifier, int totalBars, int soulPosition, int[] occupied)
    {
        int section = 0;
        int middle = totalBars / 2;

        switch (modifier.Type)
        {
            case BarModifierType.Barrier:
                section = rnd.Next(0, soulPosition);
                break;
            
            case BarModifierType.Beartrap:
                section = rnd.Next(soulPosition + 1, totalBars);
                break;
            
            case BarModifierType.GravityPull:
                section = totalBars - 1;
                break;
        }

        if (occupied.Contains(section)) return ChooseBarSection(rnd, modifier, totalBars, soulPosition, occupied);

        return section;
    }
}