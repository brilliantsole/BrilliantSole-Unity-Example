using System.Text;
using UnityEngine;

public static class BS_StringUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_StringUtils", BS_Logger.LogLevel.Log);

    public static string GetString(in byte[] data, bool includesLength = false)
    {
        var offset = 0;
        var stringLength = data.Length;
        if (includesLength) { stringLength = data[offset++]; }
        var parsedString = Encoding.UTF8.GetString(data, offset, stringLength);
        Logger.Log($"parsed string: {parsedString}");
        return parsedString;
    }
}
