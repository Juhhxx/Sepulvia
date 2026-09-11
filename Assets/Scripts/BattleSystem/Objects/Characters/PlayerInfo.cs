using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Battle System/New Player Character")]
public class PlayerInfo : CharacterInfo
{
    [field: SerializeField] public int Essence { get; private set; }
}

public class Player : Character
{
    public Player(PlayerInfo info) : base(info)
    {
        Essence = info.Essence;
    }

    [SerializeField, ReadOnly] private int _essence;
    public int Essence
    {
        get => _essence;
        set
        {
            _essence = value < 0 ? 0 : value;
            OnEssenceChange?.Invoke(_essence);
        }
    }

    public void ChangeEssence(int amount) => Essence += amount;

    public Action<int> OnEssenceChange;

    public int Level => CalculatePlayerLevel();

    private int CalculatePlayerLevel()
    {
        float statLevelsMedian = (StanceLevel + SpeedLevel) / 2f;

        float moveLevelsMedian  = 0;

        foreach (Move move in MoveSet) moveLevelsMedian += move.Level;

        moveLevelsMedian /= 4f;

        int finalLevel = Mathf.FloorToInt((statLevelsMedian + moveLevelsMedian) / 2f);

        Debug.Log($"LEVEL PLAYER: {finalLevel}");

        return finalLevel;
    }
}
