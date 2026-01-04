using System.Collections.Generic;
using UnityEngine;

public class BattleResolver : MonoBehaviour
{
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private InventoryResolver _inventoryResolver;

    private int SelectedBar => _pullManager.SelectedIndex;

    public void DoMove(MoveInfo move, CharacterInfo user, PartyInfo target)
    {
        Debug.Log($"{user.Name} USED {move.Name} AGAINST {target.PartyName}");

        user.CurrentStance -= move.StanceCost;

        foreach (CharacterInfo c in target.PartyMembers)
        {
            c.CurrentStance -= move.StanceDamage;
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

    private CharacterInfo ChooseTarget(PartyInfo fromParty)
    {
        int rnd = UnityEngine.Random.Range(0, fromParty.PartySize);
        CharacterInfo c = fromParty.PartyMembers[rnd];

        Debug.Log($"TARGETING {c.Name}");

        return c;
    }

    private void DoPull(MoveInfo move, CharacterInfo user)
    {
        DialogueManager.Instance.AddDialogue(
        $"{user.Name} pushed the Soul to their side by {move.PullStrength + user.PullStrenghtBonus}.");
        
        if (user is PlayerInfo)
        {
            _pullManager.MoveHeart(-move.PullStrength -
            user.PullStrenghtBonus);
        }
        else
        {
            _pullManager.MoveHeart(move.PullStrength +
            user.PullStrenghtBonus);
        }
    }

    private void DoStatModifier(MoveInfo move, CharacterInfo target)
    {
        foreach (StatModifier sm in move.StatModifiers)
        {
            target.AddModifier(sm.Instantiate());
            if (move.Type == MoveTypes.Buff)
            {
                DialogueManager.Instance.AddDialogue(
                $"{target.Name} {sm.StatAffected.ToTitle()} Rose.");
            }
            else if (move.Type == MoveTypes.Nerf)
            {
                DialogueManager.Instance.AddDialogue(
                $"{target.Name} {sm.StatAffected.ToTitle()} Fell.");
            }
        }
    }

    private void ApplyBarModifier(MoveInfo move)
    {
        _pullManager.BarSections[SelectedBar].AddBarModifier(move.Modifier);
    }

    public void UseItem(ItemInfo item, CharacterInfo user)
    {
        _inventoryResolver.UseItem(item, user);
    }

    public (List<ItemInfo>, int) GiveRewards(PartyInfo enemyParty, bool spared)
    {
        // Return Values
        List<ItemInfo> items = new List<ItemInfo>();
        int essence = 0;

        // Party Values
        List<ItemInfo> possibleRewards = new List<ItemInfo>();
        int totalDifficulty = 0;

        foreach (EnemyInfo e in enemyParty.PartyMembers)
        {
            totalDifficulty += e.DifficultyLevel;

            possibleRewards.AddRange(e.PossibleRewards);
        }

        // Reward Calculation
        essence = Random.Range(1, (totalDifficulty * 2) + 1) * 5;

        if (spared) essence *= 2;

        if (!spared)
        {
            int num = Random.Range(1, totalDifficulty + 1);

            for (int i = 0; i < num; i++)
            {
                int rnd = Random.Range(0, possibleRewards.Count);

                items.Add(possibleRewards[rnd]);
            }
        }
        
        return (items, essence);
    }

    public bool CanRun(PartyInfo enemyParty)
    {
        float rnd = Random.Range(0,1f);

        float difficultyAverage = 0;

        foreach (EnemyInfo e in enemyParty.PartyMembers)
        {
            difficultyAverage += e.DifficultyLevel;
        }

        difficultyAverage /= enemyParty.PartySize;

        float chance = (1 / difficultyAverage) + 0.25f;

        return rnd <= chance;
    }
}
