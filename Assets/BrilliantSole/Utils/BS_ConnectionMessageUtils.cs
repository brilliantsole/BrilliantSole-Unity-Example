using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class BS_ConnectionMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ConnectionMessageUtils");

    static public readonly ReadOnlyCollection<string> EnumStrings;
    static public readonly ReadOnlyDictionary<string, byte> EnumStringMap;
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

        EnumStrings = new(_enumStrings);
        EnumStringMap = new(stringToByte);
    }

    private static void AppendEnum<TEnum>(ref byte offset, List<string> enumStrings) where TEnum : Enum
    {
        var names = Enum.GetNames(typeof(TEnum));
        AppendStrings(ref offset, enumStrings, names);
    }

    private static void AppendStrings(ref byte offset, List<string> enumStrings, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            Logger.Log($"#{offset}: {name}");
            enumStrings.Add(name);
            stringToByte[name] = offset;
            offset++;
        }
    }

    private static readonly Dictionary<string, byte> stringToByte = new();

    public static byte[] StringArrayToByteArray(string[] strings)
    {
        byte[] byteArray = new byte[strings.Length];

        for (int i = 0; i < strings.Length; i++)
        {
            byteArray[i] = stringToByte[strings[i]];
        }

        return byteArray;
    }

    public static BS_ConnectionMessage CreateMessage(string enumString, in List<byte> data) { return new(stringToByte[enumString], data); }
    public static BS_ConnectionMessage CreateMessage(string enumString) { return new(stringToByte[enumString]); }
}
