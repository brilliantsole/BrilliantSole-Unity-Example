using System;
using UnityEngine;

public static class BS_TimeUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TimeUtils", BS_Logger.LogLevel.Log);

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

        ulong timestamp = currentTime - (currentTime % ushort.MaxValue);
        Logger.Log($"truncated timestamp: {rawTimestamp}ms");

        ulong timestampDifference = currentTime > timestamp ? currentTime - timestamp : timestamp - currentTime;
        Logger.Log($"timestampDifference: {timestampDifference}ms");
        if (timestampDifference > TimestampThreshold)
        {
            Logger.Log("correcting timestamp overflow");
            timestamp += ushort.MaxValue * (ulong)Mathf.Sign(currentTime - timestamp);
        }

        Logger.Log($"timestamp: {rawTimestamp}ms");
        return timestamp;
    }

    private static readonly ushort TimestampThreshold = 60_000;
}
