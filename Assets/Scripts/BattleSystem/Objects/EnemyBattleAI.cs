using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyBattleAI : MonoBehaviour
{
    public void ChooseRandom(BattlerController battler, BattlerController[] possibleTargets, List<BarSection> sections, int soulPosition)
    {
        System.Random rnd = new System.Random();
        Move move = null;

        bool ok = false;
        int maxIteration = 5;
        int iteration = 0;

        while (!ok && iteration < maxIteration)
        {
            move = battler.Character.MoveSet[rnd.Next(battler.Character.MoveSet.Count)];

            ok = !move.CheckIfCooldown();

            iteration++;
        }

        if (move == null)
        {
            UnityEngine.Debug.Log("MAX ITERATION REACHED, CHOOSING FIRST AVAILABLE MOVE");
            move = battler.Character.MoveSet[0];
        }

        // if (move.Type == MoveTypes.Modifier)
        // {
        //     var occupiedBars = sections.FindAll(s => s.HasModifier).Select(s => sections.IndexOf(s));

        //     if (move.Modifier.Type == BarModifierType.GravityPull && occupiedBars.Contains(0)) return ChooseRandom(sections, soulPosition);

        //     move.SetBarSection(ChooseBarSection(rnd, move.Modifier, sections.Count, soulPosition, occupiedBars.ToArray()));
        // }

        var targets = ChooseTargets(battler, possibleTargets, move.Targeting);

        battler.QueueAction(new BattleAction(battler, targets, move));
    }
    private BattlerController[] ChooseTargets(BattlerController battler, BattlerController[] possibleTargets, MoveTargeting targeting)
    {
        if (targeting == MoveTargeting.Self) return new BattlerController[1] {battler};
        else if (targeting == MoveTargeting.All) return possibleTargets;
        else if (targeting == MoveTargeting.Single)
        {
            System.Random rnd = new System.Random();
            int targetIndex = rnd.Next(0, possibleTargets.Length);

            return new BattlerController[1] {possibleTargets[targetIndex]};
        }

        return new BattlerController[0];
    }

    public int ChooseBarSection(System.Random rnd, BarModifier modifier, int totalBars, int soulPosition, int[] occupied)
    {
        int section = 0;
        int middle = totalBars / 2;

        // switch (modifier.Type)
        // {
        //     case BarModifierType.Barrier:
        //         section = rnd.Next(0, soulPosition);
        //         break;
            
        //     case BarModifierType.Beartrap:
        //         section = rnd.Next(soulPosition + 1, totalBars);
        //         break;
            
        //     case BarModifierType.GravityPull:
        //         section = totalBars - 1;
        //         break;
        // }

        if (occupied.Contains(section)) return ChooseBarSection(rnd, modifier, totalBars, soulPosition, occupied);

        return section;
    }
}