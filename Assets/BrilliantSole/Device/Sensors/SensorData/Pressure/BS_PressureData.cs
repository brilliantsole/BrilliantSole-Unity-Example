using System;
using UnityEngine;

[Serializable]
public readonly struct BS_PressureData
{
    public readonly BS_PressureSensorData[] Sensors { get; }
    public readonly float ScaledSum { get; }
    public readonly float NormalizedSum { get; }

    public readonly Vector2? CenterOfPressure { get; }
    public readonly Vector2? NormalizedCenterOfPressure { get; }

    public BS_PressureData(BS_PressureSensorData[] sensors, float scaledSum, float normalizedSum, in Vector2? centerOfPressure, in Vector2? normalizedCenterOfPressure)
    {
        Sensors = sensors;
        ScaledSum = scaledSum;
        NormalizedSum = normalizedSum;
        CenterOfPressure = centerOfPressure;
        NormalizedCenterOfPressure = normalizedCenterOfPressure;
    }
}
