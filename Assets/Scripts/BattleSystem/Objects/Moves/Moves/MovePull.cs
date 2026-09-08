using UnityEngine;

public class MovePull : IMove
{
    [SerializeField] private int _pullAmount;
    public void OnDoMove(Character user, Character[] targets, BattleResolver resolver)
    {
        resolver.DoPull(_pullAmount, user);
    }
}