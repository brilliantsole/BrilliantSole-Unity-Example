using System;

[Flags]
public enum BS_VibrationLocationFlag : byte
{
    None = 0,
    Front = 1 << 0,
    Rear = 1 << 1
}

