using System;
using UnityEngine;

[Serializable]
public readonly struct BS_PressureSensorData
{
    public readonly Vector2 Position { get; }
    public readonly int RawValue { get; }
    public readonly float ScaledValue { get; }
    public readonly float NormalizedValue { get; }
    public readonly float WeightedValue { get; }

    public override readonly string ToString()
    {
        return $"Position: ({Position.x}, {Position.y}), RawValue: {RawValue}, ScaledValue: {ScaledValue}, NormalizedValue: {NormalizedValue}, WeightedValue: {WeightedValue}";
    }
}