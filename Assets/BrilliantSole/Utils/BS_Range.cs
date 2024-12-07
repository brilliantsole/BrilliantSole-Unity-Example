using System;
using UnityEngine;

public class BS_Range
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_Range");

    public float Min { get; private set; }
    public float Max { get; private set; }
    public float Span { get; private set; }

    public void Reset()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
        Span = 0;
    }

    public BS_Range()
    {
        Reset();
    }

    public void Update(float value)
    {
        Min = Mathf.Min(Min, value);
        Max = Mathf.Max(Max, value);
        Span = Max - Min;
        Logger.Log($"updated Range to {ToString()}");
    }

    public float GetNormalization(float value, bool weightBySpan)
    {
        if (Span == 0)
        {
            return 0.0f;
        }
        float Interpolation = (value - Min) / Span;

        if (weightBySpan)
        {
            Interpolation *= Span;
        }
        return Interpolation;
    }

    public float UpdateAndGetNormalization(float value, bool weightBySpan)
    {
        Update(value);
        return GetNormalization(value, weightBySpan);
    }

    public override string ToString()
    {
        return $"Min: {Min}, Max: {Max}, Span: {Span}";
    }
}
