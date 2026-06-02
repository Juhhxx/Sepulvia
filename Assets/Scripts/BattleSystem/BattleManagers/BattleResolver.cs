using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResolver : RandomBehaviour
{
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private InventoryResolver _inventoryResolver;

    private void Start()
    {
        TryInitializeRandom();
    }

    public void DoMove(Move move, Character user, Party target)
    {
        Debug.Log($"{user.Name} USED {move.Name} AGAINST {target.PartyName}");

        user.CurrentStance -= move.StanceCost;

        foreach (Character c in target.PartyMembers)
        {
            c.CurrentStance -= move.StanceDamage;

            if (move.StanceDamage > 0)
                if (c is Player) c.Animator?.SetTrigger("Hurt");
        }

        switch(move.Type)
        {
            case MoveTypes.Pull:

                DoPull(move, user);
                break;

            case MoveTypes.Buff:

                DoStatModifier(move, user);
                break;

            case MoveTypes.Nerf:

                DoStatModifier(move, ChooseTarget(target));
                break;

            case MoveTypes.Modifier:

                ApplyBarModifier(move);
                break;
        }
    }

    private Character ChooseTarget(Party fromParty)
    {
        int rnd = _random.Next(0, fromParty.PartySize);
        Character c = fromParty.PartyMembers[rnd];

        Debug.Log($"TARGETING {c.Name}");

        return c;
    }

    private void DoPull(Move move, Character user)
    {
        int pullStrenght = move.PullStrength + user.PullStrenghtBonus;

        if (pullStrenght < 0) pullStrenght = 0;

        if (pullStrenght > 0)
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Name} pulled the Soul to their side by {move.PullStrength + user.PullStrenghtBonus}.");
        }
        else
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Name} failed to pull the Soul to their side.");
        }
       
        user.Animator?.SetTrigger("Attack");
        
        if (user is Player)
        {
            _pullManager.MoveHeart(-pullStrenght);
        }
        else
        {
            _pullManager.MoveHeart(pullStrenght);
        }
    }

    private void DoStatModifier(Move move, Character target)
    {
        foreach (StatModifier sm in move.StatModifiers)
        {
            target.AddModifier(sm.Instantiate());

            if (move.Type == MoveTypes.Buff)
            {
                if (target is Player) target.Animator?.SetTrigger("Buff");

                DialogueManager.Instance.AddDialogue(
                $"{target.Name} {sm.StatAffected.ToTitle()} Rose.");
            }
            else if (move.Type == MoveTypes.Nerf)
            {
                if (target is Player) target.Animator?.SetTrigger("Nerf");

                DialogueManager.Instance.AddDialogue(
                $"{target.Name} {sm.StatAffected.ToTitle()} Fell.");
            }
        }
    }

    private void ApplyBarModifier(Move move)
    {
        _pullManager.BarSections[move.BarSection].AddBarModifier(move.Modifier);
    }

    public void UseItem(ItemInfo item, Character user)
    {
        _inventoryResolver.UseItem(item, user);
    }

    public (List<ItemInfo>, int) GiveRewards(Party enemyParty, bool spared)
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

    public bool CanRun(Character user, Party enemyParty)
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

        if (result) DialogueManager.Instance.AddDialogue($"{user.Name} ran away!");
        else DialogueManager.Instance.AddDialogue($"{user.Name} couldn't run from battle.");

        return result;
    }
}
