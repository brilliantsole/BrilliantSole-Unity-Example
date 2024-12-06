using UnityEngine;
using System;

[Serializable]
public readonly struct BS_DevicePairPressureData
{
    public readonly float RawSum { get; }
    public readonly float NormalizedSum { get; }

    public readonly Vector2? CenterOfPressure { get; }
    public readonly Vector2? NormalizedCenterOfPressure { get; }

    public BS_DevicePairPressureData(float rawSum, float normalizedSum, in Vector2? centerOfPressure, in Vector2? normalizedCenterOfPressure)
    {
        RawSum = rawSum;
        NormalizedSum = normalizedSum;
        CenterOfPressure = centerOfPressure;
        NormalizedCenterOfPressure = normalizedCenterOfPressure;
    }
}