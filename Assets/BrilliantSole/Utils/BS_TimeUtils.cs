using System;
using UnityEngine;

public static class BS_TimeUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TimeUtils");

    public static ulong GetMilliseconds()
    {
        return (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public static ulong ParseTimestamp(byte[] data, in int offset = 0)
    {
        var currentTime = GetMilliseconds();
        Logger.Log($"currentTime: {currentTime}ms");

        var rawTimestamp = BS_ByteUtils.ParseNumber<ushort>(data, offset, true);
        Logger.Log($"rawTimestamp: {rawTimestamp}ms");

        ulong timestamp = currentTime - (currentTime % (ushort.MaxValue + 1));
        Logger.Log($"truncated timestamp: {timestamp}ms");
        timestamp += rawTimestamp;
        Logger.Log($"full timestamp: {timestamp}ms");

        ulong timestampDifference = currentTime > timestamp ? currentTime - timestamp : timestamp - currentTime;
        Logger.Log($"timestampDifference: {timestampDifference}ms");
        if (timestampDifference > TimestampThreshold)
        {
            Logger.Log("correcting timestamp overflow");
            timestamp += ushort.MaxValue * (ulong)Mathf.Sign(currentTime - timestamp);
        }

        Logger.Log($"timestamp: {timestamp}ms");
        return timestamp;
    }

    private static readonly ushort TimestampThreshold = 60_000;
}
