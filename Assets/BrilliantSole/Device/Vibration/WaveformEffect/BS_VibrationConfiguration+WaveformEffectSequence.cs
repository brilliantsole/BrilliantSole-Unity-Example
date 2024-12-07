using System.Collections.Generic;
using UnityEngine;

public partial struct BS_VibrationConfiguration
{
    private readonly List<byte> GetWaveformEffectSequenceToArray()
    {
        List<byte> Data = new();
        // FILL
        for (var i = 0; i < WaveformEffectSequence.Count && i < MaxNumberOfWaveformEffectSegments; i++)
        {
            // FILL
        }
        return Data;
    }
}
