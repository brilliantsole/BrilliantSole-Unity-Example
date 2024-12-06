using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BS_VibrationConfiguration
{
    public BS_VibrationType Type;
    public BS_VibrationLocationFlag Locations;

    // WAVEFORM EFFECT START
    public List<BS_VibrationWaveformEffectSegment> WaveformEffectSequence;

    [Range(0, 6)]
    public byte WaveformEffectSequenceLoopCount;

    static readonly byte MaxNumberOfWaveformEffectSegments = 8;
    static readonly byte MaxWaveformEffectSequenceLoopCount = 6;

    // WAVEFORM EFFECT END

    // WAVEFORM START
    public List<BS_VibrationWaveformSegment> WaveformSequence;

    static readonly byte MaxNumberOfWaveformSegments = 20;

    // WAVEFORM END

    public byte[] ToArray()
    {
        // FILL
        return new byte[] { };
    }
}
