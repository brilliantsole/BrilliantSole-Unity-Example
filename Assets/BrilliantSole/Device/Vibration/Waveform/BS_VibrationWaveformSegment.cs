using System;
using UnityEngine;

[Serializable]
public struct BS_VibrationWaveformSegment
{
    [Range(0f, 1f)]
    public float Amplitude;

    [Range(0, 2550)]
    public ushort Duration;

    static readonly ushort MaxDuration = 2550;
}