using System;
using UnityEngine;

[Serializable]
public class BattleAction
{
    [field: SerializeField] public Character Character { get; private set; }
    [field: SerializeField] public ActionType Type { get; private set;}
    [field: SerializeField] public Move Move { get; private set; }
    [field: SerializeField] public ItemInfo Item { get; private set;}

    public BattleAction(Character character, Move move)
    {
        Character = character;
        Type = ActionType.Move;
        Move = move;
        Item = null;
    }

    public BattleAction(Character character, ItemInfo item)
    {
        Character = character;
        Type = ActionType.Item;
        Move = null;
        Item = item;
    }

    public BattleAction(Character character)
    {
        Character = character;
        Type = ActionType.Run;
        Move = null;
        Item = null;
    }
}