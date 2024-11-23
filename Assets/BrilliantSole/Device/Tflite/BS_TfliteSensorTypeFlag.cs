using System;

[Flags]
public enum BS_TfliteSensorTypeFlag : byte
{
    None = 0,
    Pressure = 1 << 0,
    LinearAcceleration = 1 << 1,
    Gyroscope = 1 << 2,
    Magnetometer = 1 << 3,
}

