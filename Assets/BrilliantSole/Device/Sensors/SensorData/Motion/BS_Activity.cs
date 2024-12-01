using System;

[Flags]
public enum BS_Activity : byte
{
    None = 0,
    Still = 1 << 0,
    Walking = 1 << 1,
    Running = 1 << 2,
    Bicycle = 1 << 3,
    Vehicle = 1 << 4,
    Tilting = 1 << 5
}
