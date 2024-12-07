using System;
using UnityEngine;

[Serializable]
public struct BS_VibrationWaveformEffectSegment : ISerializationCallbackReceiver
{
    public BS_VibrationWaveformEffectSegmentType Type;
    public BS_VibrationWaveformEffect Effect;

    [Range(0, 1270)]
    public ushort Delay;

    [Range(0, 3)]
    public byte LoopCount;

    public BS_VibrationWaveformEffectSegment(BS_VibrationWaveformEffect effect, byte loopCount = 0)
    {
        Type = BS_VibrationWaveformEffectSegmentType.Effect;
        Effect = effect;
        LoopCount = loopCount;

        Delay = 0;

        OnBeforeSerialize();
    }
    public BS_VibrationWaveformEffectSegment(ushort delay, byte loopCount = 0)
    {
        Type = BS_VibrationWaveformEffectSegmentType.Delay;
        Delay = delay;
        LoopCount = loopCount;

        Effect = BS_VibrationWaveformEffect.None;

        OnBeforeSerialize();
    }

    static public readonly ushort MaxDelay = 1270;
    static public readonly byte MaxLoopCount = 3;

    public void OnBeforeSerialize()
    {
        Delay = Math.Min(Delay, MaxDelay);
        Delay -= (ushort)(Delay % 10);
        LoopCount = Math.Min(LoopCount, MaxLoopCount);
    }

    public void OnAfterDeserialize() { }
}
