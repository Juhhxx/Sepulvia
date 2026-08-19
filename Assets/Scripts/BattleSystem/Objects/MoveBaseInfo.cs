using UnityEngine;
using NaughtyAttributes;

public class MoveBaseInfo : ScriptableObject
{
    [field: Header("Base Move Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [field: Header("Move Cost Parameters")]
    [field: Space(5)]
    [field: SerializeField] public int RecoveryCost { get; private set; }
    [field: SerializeField] public int Cooldown { get; private set; }

    [field: Space(10)]
    [field: Header("Move Description")]
    [field: Space(5)]
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }
}

public class MoveBase
{
    public MoveBase(MoveBaseInfo info)
    {
        Name = info.Name;
        Icon = info.Icon;
        Level = info.Level;

        Cooldown = info.Cooldown;
        RecoveryCost = info.RecoveryCost;

        Description = info.Description;
    }

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; }
    [field: SerializeField, ReadOnly] public int Level { get; private set; }

    [field: SerializeField] public int RecoveryCost { get; private set; }

    [field: SerializeField, ReadOnly] public int Cooldown { get; private set; }
    [SerializeField, ReadOnly] private int _turnsPassed = 0;
    [SerializeField, ReadOnly] private bool _inCooldown = false;
    public void TurnPassed()
    {
        if (_inCooldown) _turnsPassed++;
        if (_turnsPassed == Cooldown + 1) // Don't cout first turn
        {
            _inCooldown = false;
            _turnsPassed = 0;
        }
    }
    public void UsedMove() => _inCooldown = true;
    public bool CheckIfCooldown() => _inCooldown;
    public void ResetCooldown()
    {
        _inCooldown = false;
        _turnsPassed = 0;
    }

    [field: SerializeField, ResizableTextArea, ReadOnly] public string Description { get; private set; }
}
