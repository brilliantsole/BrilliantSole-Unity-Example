using System;
using UnityEngine;

[Serializable]
public struct BS_VibrationWaveformEffectSegment
{
    public BS_VibrationWaveformEffectSegmentType Type;
    public BS_VibrationWaveformEffect Effect;

    [Range(0, 1270)]
    public ushort Delay;

    [Range(0, 3)]
    public byte LoopCount;

    static readonly ushort MaxDelay = 1270;
    static readonly byte MaxLoopCount = 3;
}
