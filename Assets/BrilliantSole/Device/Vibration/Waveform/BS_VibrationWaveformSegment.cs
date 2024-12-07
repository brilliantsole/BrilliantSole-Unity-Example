using System;
using UnityEngine;

[Serializable]
public struct BS_VibrationWaveformSegment : ISerializationCallbackReceiver
{
    [Range(0f, 1f)]
    public float Amplitude;

    [Range(0, 2550)]
    public ushort Duration;

    public BS_VibrationWaveformSegment(float amplitude, ushort duration)
    {
        Amplitude = amplitude;
        Duration = duration;
        OnBeforeSerialize();
    }

    static public readonly ushort MaxDuration = 2550;
    static public readonly byte AmplitudeNumberOfSteps = 127;

    public void OnBeforeSerialize()
    {
        Amplitude = Math.Clamp(Amplitude, 0, 1.0f);
        Amplitude = Mathf.RoundToInt(Amplitude * AmplitudeNumberOfSteps);
        Amplitude /= AmplitudeNumberOfSteps;

        Duration = Math.Min(Duration, MaxDuration);
        Duration -= (ushort)(Duration % 10);
    }

    public void OnAfterDeserialize() { }
}