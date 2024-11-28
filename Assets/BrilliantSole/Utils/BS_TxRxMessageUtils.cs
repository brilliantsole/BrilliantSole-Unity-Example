using System;
using UnityEngine;

public static class BS_TxRxMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TxRxMessageUtils", BS_Logger.LogLevel.Log);

    static BS_TxRxMessageUtils()
    {
        byte offset = 0;
        BS_InformationManager.InitTxRxEnum(ref offset);
        BS_SensorConfigurationManager.InitTxRxEnum(ref offset);
        BS_SensorDataManager.InitTxRxEnum(ref offset);
        BS_VibrationManager.InitTxRxEnum(ref offset);
        BS_TfliteManager.InitTxRxEnum(ref offset);
        BS_FileTransferManager.InitTxRxEnum(ref offset);
    }
}
