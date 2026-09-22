using System;

[Flags]
public enum MoveTypes
{
    Normal = 1,
    Stance = 2,
    Combo = 4,
    PartOfCombo = 8,
}

public enum MoveTargeting
{
    Single,
    All,
    Self,
}
