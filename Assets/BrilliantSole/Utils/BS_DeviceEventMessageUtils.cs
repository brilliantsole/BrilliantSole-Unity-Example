using System;
using System.Collections.Generic;

public static class BS_DeviceEventMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DeviceEventMessageUtils");

    static public readonly string[] EnumStrings;
    private static readonly byte maxDeviceEventMessageType;
    static BS_DeviceEventMessageUtils()
    {
        Logger.Log("static constructor");

        List<string> _enumStrings = new();

        byte offset = 0;
        AppendEnum<BS_ConnectionMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_ConnectionStatus>(ref offset, _enumStrings);
        AppendEnum<BS_ConnectionEventType>(ref offset, _enumStrings);
        AppendEnum<BS_MetaConnectionMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_BatteryLevelMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_InformationMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_DeviceInformationType>(ref offset, _enumStrings);
        AppendEnum<BS_DeviceInformationEventType>(ref offset, _enumStrings);
        AppendEnum<BS_SensorConfigurationMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_SensorDataMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_SensorType>(ref offset, _enumStrings);
        AppendEnum<BS_FileTransferMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_FileTransferEventType>(ref offset, _enumStrings);
        AppendEnum<BS_TfliteMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_TfliteEventType>(ref offset, _enumStrings);
        AppendEnum<BS_SmpMessageType>(ref offset, _enumStrings);
        AppendEnum<BS_SmpEventType>(ref offset, _enumStrings);
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
}
