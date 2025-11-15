using UnityEngine;

public class BattleResolver : MonoBehaviour
{
    [SerializeField] private PullingManager _pullManager;

    public void DoMove(MoveInfo move, CharacterInfo user, CharacterInfo target)
    {
        Debug.Log($"{user.Name} USED {move.Name} AGAINST {target.Name}");
        Debug.Log($"{target.Name} TOOK {move.StanceDamage} DAMAGE");

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

                // NOT IMPLEMENTED
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

    public void UseItem(ItemInfo item, CharacterInfo user)
    {
        Debug.Log($"{user.Name} USED {item.Name}");
    }
}
