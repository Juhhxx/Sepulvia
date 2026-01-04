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
}
