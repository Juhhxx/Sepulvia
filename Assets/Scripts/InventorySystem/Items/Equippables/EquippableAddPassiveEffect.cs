using System;
using UnityEngine;
public class EquippableAddPassiveEffect : IEquippable
{
    [SerializeField] private PassiveEffectInfo _passiveEffect;
    private PassiveEffect _effect;
    private Character _user;

    public void OnEquip(Character user)
    {
        _user = user;
        _effect = _passiveEffect.Instantiate();

        _user.AddPassiveEffect(_effect);
    }
    public void OnUnequip()
    {
        _user.RemovePassiveEffect(_effect);
    }
}