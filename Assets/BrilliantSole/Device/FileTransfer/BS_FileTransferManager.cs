using System;
using UnityEngine;
using static BS_FileTransferMessageType;

public class BS_FileTransferManager : BS_BaseManager<BS_FileTransferMessageType>
{
    public static readonly BS_FileTransferMessageType[] RequiredMessageTypes = {
        GetMaxFileLength,
        GetFileTransferType,
        GetFileLength,
        GetFileChecksum,
        GetFileTransferStatus,
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileTransferManager", BS_Logger.LogLevel.Warn);

    public override void OnRxMessage(BS_FileTransferMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            // FILL
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public override void Reset()
    {
        base.Reset();
        // FILL
    }
}
