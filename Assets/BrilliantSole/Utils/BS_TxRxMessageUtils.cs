using System.Collections.Generic;
using Unity.VisualScripting;

public static class BS_TxRxMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TxRxMessageUtils", BS_Logger.LogLevel.Log);

    static public readonly string[] EnumStrings;
    private static readonly byte maxTxRxMessageType;
    static BS_TxRxMessageUtils()
    {
        Logger.Log("BS_TxRxMessageUtils static constructor");

        List<string> _enumStrings = new();

        byte offset = 0;
        BS_BatteryManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_InformationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorConfigurationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorDataManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_VibrationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_TfliteManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_FileTransferManager.InitTxRxEnum(ref offset, _enumStrings);
        maxTxRxMessageType = offset;

        EnumStrings = _enumStrings.ToArray();

        RequiredTxRxMessageTypes = SetupRequiredTxRxMessageTypes();
        RequiredTxRxMessages = SetupRequiredTxRxMessages();
    }

    static public readonly byte[] RequiredTxRxMessageTypes;
    private static byte[] SetupRequiredTxRxMessageTypes()
    {
        List<byte> requiredTxMessageTypes = new();
        requiredTxMessageTypes.AddRange(BS_BatteryManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_InformationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorConfigurationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorDataManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_VibrationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_TfliteManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_FileTransferManager.RequiredTxRxMessageTypes);
        return requiredTxMessageTypes.ToArray();
    }

    static public readonly BS_TxMessage[] RequiredTxRxMessages;
    private static BS_TxMessage[] SetupRequiredTxRxMessages()
    {
        List<BS_TxMessage> requiredTxMessages = new();
        foreach (var requiredTxMessageType in RequiredTxRxMessageTypes)
        {
            requiredTxMessages.Add(new(requiredTxMessageType));
        }
        return requiredTxMessages.ToArray();
    }

}
