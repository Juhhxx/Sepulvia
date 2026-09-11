using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolver : RandomBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private InventoryResolver _inventoryResolver;

    private void Start()
    {
        TryInitializeRandom();

        // _battleManager = FindAnyObjectByType<BattleManager>();
        // _pullManager = FindAnyObjectByType<PullingManager>();
        // _inventoryResolver = FindAnyObjectByType<InventoryResolver>();

        _battleManager.OnActionExecuted += ExecuteAction;
    }

    private void ExecuteAction(BattleAction action)
    {
        switch (action.Type)
        {
            case ActionType.Move:

                DoMove(action.Move, action.User, action.Targets);
                break;

            case ActionType.Item:

                UseItem(action.Item, action.User);
                break;

            case ActionType.Run:

                _battleManager.Run();
                break;
        }
    }

    public void DoMove(Move move, Character user, Character[] targets)
    {
        int blockStun = CheckBlock(targets);

        if (blockStun > 0)
        {
            user.RecoveryTime += blockStun;

            DialogueManager.Instance.AddDialogue(
                $"{user.Name} was blocked and stunned for {blockStun} turns.");
            
            return;
        }

        Debug.Log($"{user.Name} USED {move.Name} AGAINST {targets[0].Name}");

        move.UsedMove();

        user.RecoveryTime += move.RecoveryCost;
        user.CurrentStance += GetStanceBoost(move);

        DialogueManager.Instance.AddDialogue(
            $"{user.Name} used {move.Name} against {string.Join(", ", targets.Select(t => t.Name))}.");

        move.MoveLogic.OnDoMove(user, targets, this);
    }

    private float GetStanceBoost(Move move)
    {
        float rnd = (float)_random.NextDouble();

        return Mathf.Lerp(move.StanceRewardRange.x, move.StanceRewardRange.y, rnd);
    }

    private int CheckBlock(Character[] targets)
    {
        int result = 0;

        foreach (Character c in targets)
        {
            if (c.StatusEffectManager.HasStatusEffect<StatusEffectBlock>())
            {
                var se = c.StatusEffectManager.GetStatusEffect<StatusEffectBlock>();

                result += (se.StatusEffectLogic as StatusEffectBlock).StunAmount;
                c.CurrentStance += (se.StatusEffectLogic as StatusEffectBlock).RewardAmount;
            }
        }

        return result;
    }

    public void DoPull(int strenght, Character user)
    {
        int pullStrenght = strenght + user.PullStrenghtBonus;

        if (pullStrenght < 0) pullStrenght = 0;

        if (pullStrenght > 0)
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Name} pulled the Soul to their side by {pullStrenght}.");
        }
        else
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Name} failed to pull the Soul to their side.");
        }
       
        user.Animator?.SetTrigger("Attack");
        
        Debug.Log($"{_pullManager}");
        if (user is Player)
        {
            _pullManager.MoveHeart(-pullStrenght);
        } 
        else
        {
            _pullManager.MoveHeart(pullStrenght);
        }
    }

    public void DoStatusEffect(StatusEffect statusEffect, Character[] targets)
    {
        foreach (Character target in targets)
        {
            target.StatusEffectManager.AddStatusEffect(statusEffect);
        }
    }

    public void DoBarModifier(int section, BarModifier modifier)
    {
        _pullManager.BarSections[section].AddBarModifier(modifier);
    }

    public void UseItem(ItemInfo item, Character user)
    {
        _inventoryResolver.UseItem(item, user);
    }

    public (List<ItemInfo>, int) GiveRewards(EnemyParty enemyParty, bool spared)
    {
        // Return Values
        List<ItemInfo> items = new List<ItemInfo>();
        int essence = 0;

        // Party Values
        List<ItemInfo> possibleRewards = new List<ItemInfo>();
        int totalDifficulty = 0;

        foreach (Enemy e in enemyParty.PartyMembers)
        {
            totalDifficulty += e.DifficultyLevel;

            possibleRewards.AddRange(e.PossibleRewards);
        }

        // Reward Calculation
        essence = _random.Next(1, (totalDifficulty * 2) + 1) * 5;

        if (spared) essence *= 2;

        if (!spared)
        {
            int num = _random.Next(1, totalDifficulty + 1);

            for (int i = 0; i < num; i++)
            {
                int rnd = _random.Next(0, possibleRewards.Count);

                items.Add(possibleRewards[rnd]);
            }
        }
        
        return (items, essence);
    }

    public bool CanRun(Character user, EnemyParty enemyParty)
    {
        float rnd = (float)_random.NextDouble();

        float difficultyAverage = 0;

        foreach (Enemy e in enemyParty.PartyMembers)
        {
            difficultyAverage += e.DifficultyLevel;
        }

        difficultyAverage /= enemyParty.PartySize;

        float chance = (1 / difficultyAverage) + 0.25f;

        bool result = rnd <= chance;

        if (enemyParty.PartyMembers.Any(e => !(e as Enemy).CanRun)) result = false;

        if (result) DialogueManager.Instance.AddDialogue($"{user.Name} ran away!");
        else DialogueManager.Instance.AddDialogue($"{user.Name} couldn't run from battle.");

        return result;
    }
}
