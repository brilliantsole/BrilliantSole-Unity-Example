public enum BS_FileTransferMessageType : byte
{
    GetFileTypes,
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