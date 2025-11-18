using System;

// public enum MoveTypes
// {
//     Pull,
//     Buff,
//     Nerf,
//     Modifier
// }

[Flags]
public enum MoveTypes
{
    Pull = 0x1,
    Buff = 0x10,
    Nerf = 0x100,
    Modifier = 0x1000
}