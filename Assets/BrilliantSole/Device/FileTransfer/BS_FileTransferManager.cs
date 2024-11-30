using UnityEngine;
using static BS_FileTransferMessageType;

public class BS_FileTransferManager : BS_BaseManager<BS_FileTransferMessageType>
{
    public static readonly BS_FileTransferMessageType[] RequiredMessageTypes = {
        GetFileTransferType,
        GetFileLength,
        GetFileChecksum,
        GetFileTransferStatus,
        GetFileTransferBlock,
        FileBytesTransferred
    };
    public static byte[] RequiredTxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);
}
