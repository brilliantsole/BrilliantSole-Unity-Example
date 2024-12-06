using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public struct BS_VibrationConfiguration : ISerializationCallbackReceiver
{
    public BS_VibrationType Type;
    public BS_VibrationLocationFlag Locations;

    // WAVEFORM EFFECT START
    public List<BS_VibrationWaveformEffectSegment> WaveformEffectSequence;

    [Range(0, 6)]
    public byte WaveformEffectSequenceLoopCount;

    static public readonly byte MaxNumberOfWaveformEffectSegments = 8;
    static public readonly byte MaxWaveformEffectSequenceLoopCount = 6;

    // WAVEFORM EFFECT END

    // WAVEFORM START
    public List<BS_VibrationWaveformSegment> WaveformSequence;

    static public readonly byte MaxNumberOfWaveformSegments = 20;

    // WAVEFORM END

    public byte[] ToArray()
    {
        // FILL
        return new byte[] { };
    }

    public BS_VibrationConfiguration(BS_VibrationLocationFlag locations, List<BS_VibrationWaveformEffectSegment> waveformEffectSequence, byte waveformEffectSequenceLoopCount = 0)
    {
        Type = BS_VibrationType.WaveformEffect;
        WaveformEffectSequence = waveformEffectSequence;
        WaveformSequence = new();
        Locations = locations;

        WaveformEffectSequenceLoopCount = waveformEffectSequenceLoopCount;
    }
    public BS_VibrationConfiguration(BS_VibrationLocationFlag locations, List<BS_VibrationWaveformSegment> waveformSequence)
    {
        Type = BS_VibrationType.Waveform;
        WaveformSequence = waveformSequence;
        Locations = locations;

        WaveformEffectSequence = new();
        WaveformEffectSequenceLoopCount = 0;
    }

    public void OnBeforeSerialize()
    {
        if (WaveformEffectSequenceLoopCount > MaxWaveformEffectSequenceLoopCount)
        {
            WaveformEffectSequenceLoopCount = MaxWaveformEffectSequenceLoopCount;
        }

        if (WaveformEffectSequence != null)
        {
            if (WaveformEffectSequence.Count > MaxNumberOfWaveformEffectSegments)
            {
                WaveformEffectSequence.RemoveRange(MaxNumberOfWaveformEffectSegments, WaveformEffectSequence.Count - MaxNumberOfWaveformEffectSegments);
            }
        }
        if (WaveformSequence != null)
        {
            if (WaveformSequence.Count > MaxNumberOfWaveformSegments)
            {
                WaveformSequence.RemoveRange(MaxNumberOfWaveformSegments, WaveformSequence.Count - MaxNumberOfWaveformSegments);
            }
        }
    }

    public void OnAfterDeserialize() { }
}
