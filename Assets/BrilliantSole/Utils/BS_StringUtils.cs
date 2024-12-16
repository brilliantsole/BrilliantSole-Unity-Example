using System.Collections.Generic;
using System.Text;

public static class BS_StringUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_StringUtils");

    public static string GetString(in byte[] data, bool includesLength = false)
    {
        var offset = 0;
        var stringLength = data.Length;
        if (includesLength) { stringLength = data[offset++]; }
        var parsedString = Encoding.UTF8.GetString(data, offset, stringLength);
        Logger.Log($"parsed string: {parsedString}");
        return parsedString;
    }

    public static List<byte> ToBytes(string _string, bool includeLength = false)
    {
        List<byte> bytes = new();
        if (includeLength) { bytes.Add((byte)_string.Length); }
        bytes.AddRange(Encoding.UTF8.GetBytes(_string));
        return bytes;
    }
}
