using System.Collections.Generic;

public partial struct BS_VibrationConfiguration
{
    private readonly List<byte> GetWaveformEffectSequenceToArray()
    {
        List<byte> Data = new();

        var hasAtLeast1WaveformEffectWithANonzeroLoopCount = false;
        for (var i = 0; i < WaveformEffectSequence.Count && i < MaxNumberOfWaveformEffectSegments; i++)
        {
            if (WaveformEffectSequence[i].LoopCount > 0)
            {
                hasAtLeast1WaveformEffectWithANonzeroLoopCount = true;
                break;
            }
        }

        var includeAllWaveformEffectSegments = hasAtLeast1WaveformEffectWithANonzeroLoopCount || WaveformEffectSequenceLoopCount != 0;

        for (int i = 0; i < WaveformEffectSequence.Count || (includeAllWaveformEffectSegments && i < MaxNumberOfWaveformEffectSegments); i++)
        {
            if (i >= WaveformEffectSequence.Count)
            {
                Data.Add((byte)BS_VibrationWaveformEffect.None);
                continue;
            }

            switch (WaveformEffectSequence[i].Type)
            {
                case BS_VibrationWaveformEffectSegmentType.Effect:
                    Logger.Log($"Effect #{i}: {WaveformEffectSequence[i].Effect}");
                    Data.Add((byte)WaveformEffectSequence[i].Effect);
                    break;
                case BS_VibrationWaveformEffectSegmentType.Delay:
                    Logger.Log($"Delay #{i}: {WaveformEffectSequence[i].Delay}ms");
                    Data.Add((byte)(1 << 7 | (WaveformEffectSequence[i].Delay / 10)));
                    break;
                default:
                    Logger.LogError($"uncaught waveformEffectSequenceType {WaveformEffectSequence[i].Type}");
                    break;
            }
        }

        for (int i = 0; i < WaveformEffectSequence.Count || (includeAllWaveformEffectSegments && i < MaxNumberOfWaveformEffectSegments); i++)
        {
            if (i == 0 || i == 4) { Data.Add(0); }

            var segmentLoopCount = 0;
            if (i < WaveformEffectSequence.Count) { segmentLoopCount = WaveformEffectSequence[i].LoopCount; }

            var bitOffset = 2 * (i % 4);
            Data[^1] |= (byte)(segmentLoopCount << bitOffset);

            if (i == 3 || i == 7) { }
        }

        var IncludeAllWaveformEffectSegmentLoopCounts = WaveformEffectSequenceLoopCount != 0;
        if (IncludeAllWaveformEffectSegmentLoopCounts)
        {
            Data.Add(WaveformEffectSequenceLoopCount);
        }

        return Data;
    }
}
