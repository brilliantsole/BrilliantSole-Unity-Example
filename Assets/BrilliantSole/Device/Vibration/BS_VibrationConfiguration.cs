using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public partial struct BS_VibrationConfiguration : ISerializationCallbackReceiver
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_VibrationConfiguration");

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

    public readonly List<byte> ToArray()
    {
        List<byte> Data = new()
        {
            (byte)Locations,
            (byte)Type
        };
        var VibrationData = GetVibrationData();
        if (VibrationData.Count == 0)
        {
            Logger.Log("no data in vibration data");
            Data.Clear();
            return Data;
        }
        Data.Add((byte)VibrationData.Count);
        Data.AddRange(VibrationData);
        return Data;
    }

    private readonly List<byte> GetVibrationData()
    {
        return Type switch
        {
            BS_VibrationType.WaveformEffect => GetWaveformEffectSequenceToArray(),
            BS_VibrationType.Waveform => GetWaveformSequenceToArray(),
            _ => throw new NotImplementedException()
        };
    }

    public BS_VibrationConfiguration(BS_VibrationLocationFlag locations, List<BS_VibrationWaveformEffectSegment> waveformEffectSequence, byte waveformEffectSequenceLoopCount = 0)
    {
        Type = BS_VibrationType.WaveformEffect;
        WaveformEffectSequence = waveformEffectSequence;
        WaveformSequence = new();
        Locations = locations;

        WaveformEffectSequenceLoopCount = waveformEffectSequenceLoopCount;

        OnBeforeSerialize();
    }
    public BS_VibrationConfiguration(BS_VibrationLocationFlag locations, List<BS_VibrationWaveformSegment> waveformSequence)
    {
        Type = BS_VibrationType.Waveform;
        WaveformSequence = waveformSequence;
        Locations = locations;

        WaveformEffectSequence = new();
        WaveformEffectSequenceLoopCount = 0;

        OnBeforeSerialize();
    }

    public void OnBeforeSerialize()
    {
        WaveformEffectSequenceLoopCount = Math.Min(WaveformEffectSequenceLoopCount, MaxWaveformEffectSequenceLoopCount);

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

    public readonly void OnAfterDeserialize() { }
}
