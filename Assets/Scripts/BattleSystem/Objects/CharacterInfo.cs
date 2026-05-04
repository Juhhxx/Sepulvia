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

[Serializable]
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
    // Base Speed
    [SerializeField] private int _baseSpeed;
    public int Speed => _baseSpeed + GetModifierBonus(Stats.Speed) + GetEquipmentBonus(Stats.Speed) + GetStatLevelBonus(Stats.Speed);
    private int _speedLevel = 1;
    public int SpeedLevel => _speedLevel;

    // Base Stance
    [SerializeField] private int _baseStance;
    public int MaxStance => _baseStance + GetModifierBonus(Stats.Stance) + GetEquipmentBonus(Stats.Stance) + GetStatLevelBonus(Stats.Stance);
    private int _stanceLevel = 1;
    public int StanceLevel => _stanceLevel;

    public void SetBaseStance(int stance)
    {
        _baseStance = stance;
    }

    // Current Stance
    [SerializeField, ReadOnly] private int _currentStance;
    public int CurrentStance
    {
        get => _currentStance;
        set
        {
            var tmp = _currentStance;

            if (value > MaxStance) _currentStance = MaxStance;
            else if (value <= 0) _currentStance = 0;
            else _currentStance = value;

            if(tmp != _currentStance) OnStanceChange?.Invoke(_currentStance, MaxStance, tmp);

            if (_currentStance == 0) OnStanceLost?.Invoke();
        }
    }

    public Action<int, int, int> OnStanceChange;
    public Action OnStanceLost;

    // Stance Recover
    [SerializeField] private int _baseStanceRecover;
    public int StanceRecover => _baseStanceRecover + GetModifierBonus(Stats.StanceGain) + GetEquipmentBonus(Stats.StanceGain);

    // Pull Strength
    public int PullStrenghtBonus => GetModifierBonus(Stats.PullStrength) + GetEquipmentBonus(Stats.PullStrength);

    // Modifiers
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

    // Equipment
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

    // Stat Levels
    private int _maxStatLevel = 10;

    public (int, int) GetStatLevelValue(Stats stat)
    {
        int level = 0;
        int amount = 0;

        switch (stat)
        {
            case Stats.Speed:
                level = _speedLevel;
                amount = Speed;
                break;
            case Stats.Stance:
                level = _stanceLevel;
                amount = MaxStance;
                break;
        }

        return (level, amount);
    }
    public void LevelUpStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.Speed:
                if (_speedLevel < _maxStatLevel) _speedLevel++;
                break;
            case Stats.Stance:
                if (_stanceLevel < _maxStatLevel)
                {
                    _stanceLevel++;
                    CurrentStance = MaxStance;
                }
                break;
        }
    }

    public (int, int) LevelUpCost(Stats stat)
    {
        int cost = 0;
        int amountGained = 0;

        switch (stat)
        {
            case Stats.Speed:
                cost = LevelUpCostFormula(_speedLevel, _baseSpeed, 50);
                amountGained = LevelBonusFormula(_speedLevel + 1, _baseSpeed, 50) - LevelBonusFormula(_speedLevel, _baseSpeed, 50);
                break;
            case Stats.Stance:
                cost = LevelUpCostFormula(_stanceLevel, _baseStance, 100);
                amountGained = LevelBonusFormula(_stanceLevel + 1, _baseStance, 100) - LevelBonusFormula(_stanceLevel, _baseStance, 100);
                break;
        }

        return (cost, amountGained);
    }

    private int GetStatLevelBonus(Stats stat)
    {
        int bonus = 0;

        switch (stat)
        {
            case Stats.Speed:
                bonus = LevelBonusFormula(_speedLevel, _baseSpeed, 50); // Assuming 50 is the max amount for Speed
                break;
            case Stats.Stance:
                bonus = LevelBonusFormula(_stanceLevel, _baseStance, 100); // Assuming 100 is the max amount for Stance
                break;
        }

        return bonus;
    }

    private int LevelBonusFormula(int level, int baseAmount, int maxAmount)
    {
        if (level == 1) return 0;
        
        float bonusf = RawLevelBonusFormula(level, baseAmount, maxAmount);
        int bonus = Mathf.RoundToInt(bonusf / 5f) * 5; // Round to nearest 5

        return bonus - baseAmount;
    }
    private float RawLevelBonusFormula(int level, int baseAmount, int maxAmount)
    {
        float t = (level - 1f) / (_maxStatLevel - 1f);
        float bonus = baseAmount * Mathf.Pow(maxAmount / baseAmount, t);

        return bonus;
    }

    private int LevelUpCostFormula(int level, int baseAmount, int maxAmount)
    {
        int baseCost = 5; // base cost for amount gained
        float t = 1.3f; 

        float difference = RawLevelBonusFormula(level + 1, baseAmount, maxAmount) - RawLevelBonusFormula(level, baseAmount, maxAmount);
        float costf = baseCost * Mathf.Pow(difference, t);

        return Mathf.RoundToInt(costf / 5f) * 5; // Round to nearest 5
    }

    [field: Space(10)]
    [field: Header("Character Moves")]
    [field: Space(5)]
    [field: SerializeField] public List<Move> MoveSet { get; private set; }
    private List<Move> _baseMoves;
    public void SetBaseMoves() => _baseMoves = new List<Move>(MoveSet);
    public void ResetMoves() => MoveSet = new List<Move>(_baseMoves);
    public void ResetMove(int index) => MoveSet[index] = _baseMoves[index];
    public void ChangeMove(int index, Move to) => MoveSet[index] = to;

    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField] public Inventory Inventory { get; private set; }
}
