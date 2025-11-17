using UnityEngine;

public class BattleResolver : MonoBehaviour
{
    [SerializeField] private PullingManager _pullManager;

    private int SelectedBar => _pullManager.SelectedIndex;

    public void DoMove(MoveInfo move, CharacterInfo user, CharacterInfo target)
    {
        Debug.Log($"{user.Name} USED {move.Name} AGAINST {target.Name}");

        user.CurrentStance -= move.StanceCost;
        target.CurrentStance -= move.StanceDamage;

        switch(move.Type)
        {
            case MoveTypes.Pull:

                DoPull(move, user);
                break;

            case MoveTypes.Buff:

                DoStatModifier(move, user);
                break;

            case MoveTypes.Nerf:

                DoStatModifier(move, target);
                break;

            case MoveTypes.Modifier:

                ApplyBarModifier(move);
                break;
        }
    }

    private void DoPull(MoveInfo move, CharacterInfo user)
    {
        if (user is PlayerInfo)
        {
            _pullManager.MoveHeart(-move.PullStrength -
            user.GetModifierBonus(Stats.PullStrength));
        }
        else
        {
            _pullManager.MoveHeart(move.PullStrength +
            user.GetModifierBonus(Stats.PullStrength));
        }
    }

    private void DoStatModifier(MoveInfo move, CharacterInfo target)
    {
        foreach (StatModifier sm in move.StatModifiers) target.AddModifier(sm.Instantiate());
    }

    private void ApplyBarModifier(MoveInfo move)
    {
        _pullManager.BarSections[SelectedBar].AddBarModifier(move.Modifier);
    }

    public void UseItem(ItemInfo item, CharacterInfo user)
    {
        Debug.Log($"{user.Name} USED {item.Name}");

        switch (item.Type)
        {
            case ItemTypes.Immediate:

                ApplyItemEffect(item, user);
                break;
            
            case ItemTypes.LongTerm:

                user.AddModifier(item.Modifier.Instantiate());
                break;
        }
    }

    private void ApplyItemEffect (ItemInfo item, CharacterInfo user)
    {
        switch (item.Stat)
        {
            case Stats.Stance:

                user.CurrentStance += item.Amount;
                break;
            
            default:
                
                break;
        }
    }
}
