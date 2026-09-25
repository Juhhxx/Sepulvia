using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public class EnemyBattleAI : MonoBehaviour
{
    private System.Random _rnd = new System.Random();
    private List<BarSection> _barSections;
    private int _currentSoulPosition;

    public void ChooseRandom(BattlerController battler, BattlerController[] allies, BattlerController[] possibleTargets, List<BarSection> sections, int soulPosition)
    {
        _barSections = sections;
        _currentSoulPosition = soulPosition;
        
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

        var targets = ChooseTargets(battler, allies, possibleTargets, move.Targeting);

        Debug.Log("ENEMY CHOSE MOVE");

        battler.QueueAction(new BattleAction(battler, targets, move));
    }
    public BattlerController[] ChooseTargets(BattlerController battler, BattlerController[] allies, BattlerController[] possibleTargets, MoveTargeting targeting)
    {
        System.Random rnd = new System.Random();
        BattlerController[] targets = null;

        var alliesNoUser = new List<BattlerController>();
        alliesNoUser.Remove(battler);

        switch (targeting)
        {
            case MoveTargeting.Single:

                int targetIndex = rnd.Next(0, possibleTargets.Length);
                targets = new BattlerController[1] {possibleTargets[targetIndex]};
                break;
            
            case MoveTargeting.All:

                targets = possibleTargets;
                break;
            
            case MoveTargeting.Self:

                targets = new BattlerController[1] {battler};
                break;
            
            case MoveTargeting.AllySingle:

                int allyIndex = rnd.Next(0, alliesNoUser.Count);
                targets = new BattlerController[1] {alliesNoUser[allyIndex]};
                break;
            
            case MoveTargeting.AllyAll:

                targets = alliesNoUser.ToArray();
                break;

            case MoveTargeting.Everyone:

                var everyone = new List<BattlerController>(allies);
                everyone.AddRange(possibleTargets);

                targets = everyone.ToArray();
                break;
            
            case MoveTargeting.None:

                targets = new BattlerController[0];
                break;
        }

        return targets;
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