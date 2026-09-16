using System;
using UnityEngine;

[Serializable]
public class BattleAction
{
    [field: SerializeField] public Character User { get; private set; }
    [field: SerializeField] public Character[] Targets { get; private set; }
    [field: SerializeField] public ActionType Type { get; private set;}
    [field: SerializeField] public Move Move { get; private set; }
    [field: SerializeField] public ItemInfo Item { get; private set;}

    public BattleAction(Character user, Character[] targets, Move move)
    {
        User = user;
        Targets = targets;
        Type = ActionType.Move;
        Move = move;
        Item = null;
    }

    public BattleAction(Character user, ItemInfo item)
    {
        User = user;
        Targets = null;
        Type = ActionType.Item;
        Move = null;
        Item = item;
    }

    // Empty action is run
    public BattleAction(Character user)
    {
        User = user;
        Targets = null;
        Type = ActionType.Run;
        Move = null;
        Item = null;
    }
}