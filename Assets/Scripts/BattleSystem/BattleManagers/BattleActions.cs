using System;
using UnityEngine;

[Serializable]
public class BattleAction
{
    [field: SerializeField] public Character Character { get; private set; }
    [field: SerializeField] public Character[] Targets { get; private set; }
    [field: SerializeField] public ActionType Type { get; private set;}
    [field: SerializeField] public Move Move { get; private set; }
    [field: SerializeField] public ItemInfo Item { get; private set;}

    public BattleAction(Character character, Character[] targets, Move move)
    {
        Character = character;
        Targets = targets;
        Type = ActionType.Move;
        Move = move;
        Item = null;
    }

    public BattleAction(Character character, ItemInfo item)
    {
        Character = character;
        Targets = null;
        Type = ActionType.Item;
        Move = null;
        Item = item;
    }

    // Empty action is run
    public BattleAction(Character character)
    {
        Character = character;
        Targets = null;
        Type = ActionType.Run;
        Move = null;
        Item = null;
    }
}