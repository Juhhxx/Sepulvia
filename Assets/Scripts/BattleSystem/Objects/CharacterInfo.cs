using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInfo : ScriptableObject
{
    [field: Header("Character Cosmetics")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject BattlePrefab { get; private set; }

    [Space(10)]
    [Header("Character Stats")]
    [Space(5)]
    [SerializeField] private int _baseSpeed;
    public int Speed => _baseSpeed;

    [SerializeField] private int _baseStance;
    public int MaxStance => _baseStance;

    [SerializeField] private int _baseStanceRecover;
    public int StanceRecover => _baseStanceRecover;

    [field: Space(10)]
    [field: Header("Character Moves")]
    [field: Space(5)]
    [field: SerializeField, Expandable] public List<MoveInfo> MoveSet { get; private set; }

    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField, Expandable] public InventoryInfo Inventory { get; private set; }

    public Character Instantiate()
    {
        return this is PlayerInfo ? new Player(this as PlayerInfo) : new Enemy(this as EnemyInfo);
    }
}

public class Character
{
    public Character(CharacterInfo info)
    {
        Name = info.Name;
        BattlePrefab = info.BattlePrefab;

        _baseSpeed = info.Speed;
        _baseStance = info.MaxStance;
        _currentStance = info.MaxStance;
        _baseStanceRecover = info.StanceRecover;
        

        _statModifiers = new List<StatModifier>();

        MoveSet = new List<Move>();

        foreach (MoveInfo m in info.MoveSet)
        {
            MoveSet.Add(m.Instantiate());
        }

        SetBaseMoves();

        CheckEquipment();

        Inventory = info.Inventory?.Instantiate();
    }

    [field: Header("Character Cosmetics")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject BattlePrefab { get; private set; }
    public Animator Animator;

    [Space(10)]
    [Header("Character Stats")]
    [Space(5)]
    [SerializeField] private int _baseSpeed;
    public int Speed => _baseSpeed + GetModifierBonus(Stats.Speed) + GetEquipmentBonus(Stats.Speed);

    [SerializeField] private int _baseStance;
    public int MaxStance => _baseStance + GetModifierBonus(Stats.Stance) + GetEquipmentBonus(Stats.Stance);

    public void SetBaseStance(int stance)
    {
        _baseStance = stance;
    }

    [SerializeField, ReadOnly] private int _currentStance;
    public int CurrentStance
    {
        get => _currentStance;
        set
        {
            if (value > MaxStance) _currentStance = MaxStance;
            else if (value <= 0) _currentStance = 0;
            else _currentStance = value;

            if (_currentStance == 0) OnStanceLost?.Invoke();
        }
    }

    public Action OnStanceLost;

    [SerializeField] private int _baseStanceRecover;
    public int StanceRecover => _baseStanceRecover + GetModifierBonus(Stats.StanceGain) + GetEquipmentBonus(Stats.StanceGain);

    public int PullStrenghtBonus => GetModifierBonus(Stats.PullStrength) + GetEquipmentBonus(Stats.PullStrength);

    [Space(10)]
    [Header("Character Stats Modifiers")]
    [Space(5)]
    [SerializeField, ReadOnly] private List<StatModifier> _statModifiers;
    public List<StatModifier> StatModifiers => _statModifiers;

    public void ResetModifiers() => _statModifiers.Clear();
    public void AddModifier(StatModifier modifier)
    {
        _statModifiers.Add(modifier);
    }
    public void CheckModifier()
    {
        foreach (StatModifier m in _statModifiers)
        {
            m.TurnPassed();
        }

        _statModifiers.RemoveAll(m => m.CheckIfDone());
    }
    public int GetModifierBonus(Stats stat)
    {
        if (_statModifiers.Count == 0) return 0;

        int bonus = 0;

        foreach (StatModifier m in _statModifiers)
        {
            if (m.StatAffected == stat) bonus += m.AmountAffected;
        }

        return bonus;
    }

    public void CheckEquipment()
    {
        if (Inventory == null) return;

        if (Inventory.EquipmentSlots.Count == 0) return;

        foreach (ItemInfo e in Inventory.EquipmentSlots)
        {
            if (e.EquipmentType == EquipmentType.MoveModidier)
            {
                MoveSet[e.MoveIndex] = e.ChangeTo.Instantiate();
            }
        }
    }
    public int GetEquipmentBonus(Stats stat)
    {
        if (Inventory == null) return 0;

        if (Inventory.EquipmentSlots.Count == 0) return 0;

        int bonus = 0;

        foreach (ItemInfo e in Inventory.EquipmentSlots)
        {
            if (e.EquipmentType == EquipmentType.StatModifier && e.StatEquip == stat)
                bonus += e.AmountEquip;
        }

        return bonus;
    }

    [field: Space(10)]
    [field: Header("Character Moves")]
    [field: Space(5)]
    [field: SerializeField, Expandable] public List<Move> MoveSet { get; private set; }
    private List<Move> _baseMoves;
    public void SetBaseMoves() => _baseMoves = new List<Move>(MoveSet);
    public void ResetMoves() => MoveSet = new List<Move>(_baseMoves);
    public void ResetMove(int index) => MoveSet[index] = _baseMoves[index];
    public void ChangeMove(int index, Move to) => MoveSet[index] = to;

    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField, Expandable] public InventoryInfo Inventory { get; private set; }
}
