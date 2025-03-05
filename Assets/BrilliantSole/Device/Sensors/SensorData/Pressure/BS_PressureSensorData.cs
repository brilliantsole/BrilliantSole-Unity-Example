using System;
using UnityEngine;

[Serializable]
public struct BS_PressureSensorData
{
    public Vector2 Position;
    public readonly int RawValue { get; }
    public readonly float ScaledValue { get; }
    public readonly float NormalizedValue { get; }
    public float WeightedValue;


    public BS_PressureSensorData(in Vector2 position, int rawValue, float scaledValue, float normalizedValue, float weightedValue)
    {
        Position = position;
        RawValue = rawValue;
        ScaledValue = scaledValue;
        NormalizedValue = normalizedValue;
        WeightedValue = weightedValue;
    }



    public override readonly string ToString()
    {
        return $"Position: ({Position.x}, {Position.y}), RawValue: {RawValue}, ScaledValue: {ScaledValue}, NormalizedValue: {NormalizedValue}, WeightedValue: {WeightedValue}";
    }
}