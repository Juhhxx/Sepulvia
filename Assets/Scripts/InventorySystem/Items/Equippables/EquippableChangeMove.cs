using System;
using UnityEngine;
using NaughtyAttributes;
public class EquippableChangeMove : IEquippable
{
    [ShowIf("_type", MoveTypes.Normal)]
    [SerializeField] private int _moveIndex;
    [SerializeField] private MoveTypes _type;
    [SerializeField] private MoveInfo _newMove;
    private Character _user;

    public void OnEquip(Character user)
    {
        _user = user;

        _user.ChangeMove(_moveIndex, _newMove.Instantiate(), _type);
    }
    public void OnUnequip()
    {
        _user.ResetMove(_moveIndex, _type);
    }
}