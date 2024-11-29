using System;
using System.Collections.Generic;
using UnityEngine;

public static class BS_TxRxMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TxRxMessageUtils", BS_Logger.LogLevel.Log);

    private static readonly byte maxTxRxMessageType;

    static BS_TxRxMessageUtils()
    {
        byte offset = 0;
        BS_InformationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorConfigurationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorDataManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_VibrationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_TfliteManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_FileTransferManager.InitTxRxEnum(ref offset, _enumStrings);
        maxTxRxMessageType = offset;
    }

    static private readonly List<string> _enumStrings = new();
    static public IReadOnlyList<string> EnumStrings => _enumStrings.AsReadOnly();
}
