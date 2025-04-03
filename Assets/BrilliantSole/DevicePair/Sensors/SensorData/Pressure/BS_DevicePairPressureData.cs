using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public readonly struct BS_DevicePairPressureData
{
    public readonly Dictionary<BS_Side, BS_PressureSensorData[]> Sensors { get; }

    public readonly float ScaledSum { get; }
    public readonly float NormalizedSum { get; }

    public readonly Vector2? CenterOfPressure { get; }
    public readonly Vector2? NormalizedCenterOfPressure { get; }

    public BS_DevicePairPressureData(Dictionary<BS_Side, BS_PressureSensorData[]> sensors, float scaledSum, float normalizedSum, in Vector2? centerOfPressure, in Vector2? normalizedCenterOfPressure)
    {
        Sensors = sensors;
        ScaledSum = scaledSum;
        NormalizedSum = normalizedSum;
        CenterOfPressure = centerOfPressure;
        NormalizedCenterOfPressure = normalizedCenterOfPressure;
    }
}