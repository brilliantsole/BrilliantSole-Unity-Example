using System;
using System.Collections.Generic;

public static class BS_ConnectionMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ConnectionMessageUtils");

    static public readonly string[] EnumStrings;
    private static readonly byte maxDeviceEventMessageType;
    static BS_ConnectionMessageUtils()
    {
        Logger.Log("static constructor");

        List<string> _enumStrings = new();

        byte offset = 0;
        AppendEnum<BS_BatteryLevelMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_DeviceInformationType>(ref offset, _enumStrings);
        AppendEnum<BS_MetaConnectionMessageType>(ref offset, _enumStrings);
        AppendStrings(ref offset, _enumStrings, BS_TxRxMessageUtils.EnumStrings);
        AppendEnum<BS_SmpMessageType>(ref offset, _enumStrings);
        maxDeviceEventMessageType = offset;

        EnumStrings = _enumStrings.ToArray();
    }

    private static void AppendEnum<TEnum>(ref byte offset, List<string> enumStrings) where TEnum : Enum
    {
        var names = Enum.GetNames(typeof(TEnum));
        foreach (var name in names)
        {
            enumStrings.Add(name);
            offset++;
        }
    }

    private static void AppendStrings(ref byte offset, List<string> enumStrings, string[] strings)
    {
        enumStrings.AddRange(strings);
        offset += (byte)strings.Length;
    }
}
