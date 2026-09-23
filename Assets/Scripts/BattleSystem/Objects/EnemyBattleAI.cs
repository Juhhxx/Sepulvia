using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyBattleAI : MonoBehaviour
{
    private System.Random _rnd = new System.Random();
    private List<BarSection> _barSections;
    private int _currentSoulPosition;

    public void ChooseRandom(BattlerController battler, BattlerController[] possibleTargets, List<BarSection> sections, int soulPosition)
    {
        Move move = null;

        bool ok = false;
        int maxIteration = 5;
        int iteration = 0;

        while (!ok && iteration < maxIteration)
        {
            move = battler.Character.MoveSet[_rnd.Next(battler.Character.MoveSet.Count)];

            ok = !move.CheckIfCooldown();

            iteration++;
        }

        if (move == null)
        {
            UnityEngine.Debug.Log("MAX ITERATION REACHED, CHOOSING FIRST AVAILABLE MOVE");
            move = battler.Character.MoveSet[0];
        }

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

    public int ChooseBarSection(ModifierApplyType whereToApply)
    {
        int totalBars = _barSections.Count;
        int section = 0;
        int middle = totalBars / 2;

        var occupied = _barSections.FindAll(s => s.HasModifier).Select(s => _barSections.IndexOf(s));

        switch (whereToApply)
        {
            case ModifierApplyType.BetweenSoulAndLeft:
                section = _rnd.Next(0, _currentSoulPosition);
                break;
            
            case ModifierApplyType.BetweenSoulAndRight:
                section = _rnd.Next(_currentSoulPosition + 1, totalBars);
                break;

            case ModifierApplyType.Anywhere:
                section = _rnd.Next(0, totalBars);
                break;
            
            case ModifierApplyType.LeftSide:
                section = 0;
                break;
            
            case ModifierApplyType.RightSide:
                section = totalBars - 1;
                break;
        }

        if (occupied.Contains(section)) return ChooseBarSection(whereToApply);

        return section;
    }
}