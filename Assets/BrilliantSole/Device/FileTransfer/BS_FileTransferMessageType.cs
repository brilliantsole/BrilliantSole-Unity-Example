public enum BS_FileTransferMessageType : byte
{
    GetFileTransferType,
    SetFileTransferType,
    GetFileLength,
    SetFileLength,
    GetFileChecksum,
    SetFileChecksum,
    SetFileTransferCommand,
    GetFileTransferStatus,
    SetFileTransferStatus,
    GetFileTransferBlock,
    SetFileTransferBlock,
    FileBytesTransferred
}