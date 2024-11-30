using UnityEngine;
using static BS_FileTransferMessageType;

public class BS_FileTransferManager : BS_BaseManager<BS_FileTransferMessageType>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileTransferManager", BS_Logger.LogLevel.Log);

    public static readonly BS_FileTransferMessageType[] RequiredMessageTypes = {
        GetFileTransferType,
        GetFileLength,
        GetFileChecksum,
        GetFileTransferStatus,
        GetFileTransferBlock,
        FileBytesTransferred
    };
    public static byte[] RequiredTxRxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);

    public override void OnRxMessage(BS_FileTransferMessageType messageType, byte[] data)
    {
        base.OnRxMessage(messageType, data);
        // FILL
    }
}
