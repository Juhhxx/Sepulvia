using UnityEngine;

public class MovePull : IMove
{
    [SerializeField] private int _pullAmount;
    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        resolver.DoPull(_pullAmount, user);
    }
}