using System.Collections.Generic;

public static class BS_TxRxMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TxRxMessageUtils", BS_Logger.LogLevel.Log);

    private static readonly byte maxTxRxMessageType;
    static BS_TxRxMessageUtils()
    {
        Logger.Log("BS_TxRxMessageUtils static constructor");

        byte offset = 0;
        BS_InformationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorConfigurationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorDataManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_VibrationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_TfliteManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_FileTransferManager.InitTxRxEnum(ref offset, _enumStrings);
        maxTxRxMessageType = offset;

        SetupRequiredTxMessageTypes();
    }

    static private readonly List<string> _enumStrings = new();
    static public IReadOnlyList<string> EnumStrings => _enumStrings.AsReadOnly();

    static private readonly List<byte> requiredTxMessageTypes = new();
    private static void SetupRequiredTxMessageTypes()
    {
        requiredTxMessageTypes.AddRange(BS_InformationManager.RequiredTxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorConfigurationManager.RequiredTxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorDataManager.RequiredTxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_VibrationManager.RequiredTxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_TfliteManager.RequiredTxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_FileTransferManager.RequiredTxMessageTypes);
        Logger.Log($"requiredTxMessageTypes: {requiredTxMessageTypes.Count}");
    }
    static public IReadOnlyList<byte> RequiredTxMessageTypes => requiredTxMessageTypes;
}
