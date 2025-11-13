using UnityEngine;

public class BattleResolver : MonoBehaviour
{
    [SerializeField] private PullingManager _pullManager;

    public void DoMove(MoveInfo move, CharacterInfo user, CharacterInfo target)
    {
        Debug.Log($"{user.Name} USED {move.Name} AGAINST {target.Name}");

        if (user.IsPlayer) _pullManager.MoveHeart(-move.PullStrength);
        else _pullManager.MoveHeart(move.PullStrength);

        user.CurrentStance -= move.StanceCost;
        target.CurrentStance -= move.StanceDamage;
    }

    public void UseItem(ItemInfo item, CharacterInfo user)
    {
        Debug.Log($"{user.Name} USED {item.Name}");
    }
}
