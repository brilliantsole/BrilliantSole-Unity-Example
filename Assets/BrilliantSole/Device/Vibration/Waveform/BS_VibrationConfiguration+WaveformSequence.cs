using System;
using System.Collections.Generic;
using UnityEngine;

public partial struct BS_VibrationConfiguration
{
    private readonly List<byte> GetWaveformSequenceToArray()
    {
        List<byte> Data = new();
        for (var i = 0; i < WaveformSequence.Count && i < MaxNumberOfWaveformSegments; i++)
        {
            Data.Add((byte)Math.Floor(WaveformSequence[i].Amplitude * BS_VibrationWaveformSegment.AmplitudeNumberOfSteps));
            Data.Add((byte)Math.Floor(WaveformSequence[i].Duration / 10.0f));
        }
        return Data;
    }
}
