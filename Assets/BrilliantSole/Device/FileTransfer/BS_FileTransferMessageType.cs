public enum BS_FileTransferMessageType : byte
{
    GetMaxFileLength,
    GetFileTransferType,
    SetFileTransferType,
    GetFileLength,
    SetFileLength,
    GetFileChecksum,
    SetFileChecksum,
    SetFileTransferCommand,
    GetFileTransferStatus,
    GetFileTransferBlock,
    SetFileTransferBlock,
    FileBytesTransferred
}