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

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileTransferManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_FileTransferMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetMaxFileLength:
                ParseMaxFileLength(data);
                break;
            case GetFileTransferType:
            case SetFileTransferType:
                ParseFileTransferType(data);
                break;
            case GetFileLength:
            case SetFileLength:
                ParseFileLength(data);
                break;
            case GetFileChecksum:
            case SetFileChecksum:
                ParseFileChecksum(data);
                break;
            case GetFileTransferStatus:
                ParseFileTransferStatus(data);
                break;
            case GetFileTransferBlock:
                ParseFileTransferBlock(data);
                break;
            case SetFileTransferBlock:
                break;
            case FileBytesTransferred:
                ParseFileBytesTransferred(data);
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public ushort? MTU;

    [SerializeField]
    private ushort? _maxFileLength;
    public ushort MaxFileLength
    {
        get => _maxFileLength ?? 0;
        private set
        {
            if (_maxFileLength == value) { return; }
            Logger.Log($"Updating MaxFileLength to {value}");
            _maxFileLength = value;
            OnMaxFileLength?.Invoke(MaxFileLength);
        }
    }
    public event Action<ushort> OnMaxFileLength;
    private void ParseMaxFileLength(in byte[] data)
    {
        var maxFileLength = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"Parsed maxFileLength: {maxFileLength}");
        MaxFileLength = maxFileLength;
    }

    [SerializeField]
    private BS_FileType? _fileType;
    public BS_FileType FileType
    {
        get => _fileType ?? 0;
        private set
        {
            if (_fileType == value) { return; }
            Logger.Log($"Updating FileType to {value}");
            _fileType = value;
            OnFileType?.Invoke(FileType);
        }
    }
    public event Action<BS_FileType> OnFileType;
    private void ParseFileTransferType(in byte[] data)
    {
        var fileType = (BS_FileType)data[0];
        Logger.Log($"Parsed fileType: {fileType}");
        FileType = fileType;
    }

    [SerializeField]
    private uint? _fileLength;
    public uint FileLength
    {
        get => _fileLength ?? 0;
        private set
        {
            if (_fileLength == value) { return; }
            Logger.Log($"Updating FileLength to {value}");
            _fileLength = value;
            OnFileLength?.Invoke(FileLength);

            // FILL
            // if (FileTransferStatus == EBS_FileTransferStatus::RECEIVING)
            // {
            //     FileToReceive.Reset(FileLength);
            // }
        }
    }
    public event Action<uint> OnFileLength;
    private void ParseFileLength(in byte[] data)
    {
        var fileLength = BS_ByteUtils.ParseNumber<uint>(data, isLittleEndian: true);
        Logger.Log($"Parsed fileLength: {fileLength}");
        FileLength = fileLength;
    }

    [SerializeField]
    private uint? _fileChecksum;
    public uint FileChecksum
    {
        get => _fileChecksum ?? 0;
        private set
        {
            if (_fileChecksum == value) { return; }
            Logger.Log($"Updating FileChecksum to {value}");
            _fileChecksum = value;
            OnFileChecksum?.Invoke(FileChecksum);
        }
    }
    public event Action<uint> OnFileChecksum;
    private void ParseFileChecksum(in byte[] data)
    {
        var fileChecksum = BS_ByteUtils.ParseNumber<uint>(data, isLittleEndian: true);
        Logger.Log($"Parsed fileChecksum: {fileChecksum}");
        FileChecksum = fileChecksum;
    }

    [SerializeField]
    private BS_FileTransferStatus? _fileTransferStatus;
    public BS_FileTransferStatus FileTransferStatus
    {
        get => _fileTransferStatus ?? 0;
        private set
        {
            if (_fileTransferStatus == value) { return; }
            Logger.Log($"Updating FileTransferStatus to {value}");
            _fileTransferStatus = value;
            OnFileTransferStatus?.Invoke(FileTransferStatus);

            // BytesTransferred = 0;

            // if (FileTransferStatus == EBS_FileTransferStatus::SENDING)
            // {
            //     UE_LOGFMT(LogBS_FileTransferManager, Verbose, "Starting to send file...");
            //     SendFileBlock(true);
            // }
        }
    }
    public event Action<BS_FileTransferStatus> OnFileTransferStatus;
    private void ParseFileTransferStatus(in byte[] data)
    {
        var fileTransferStatus = (BS_FileTransferStatus)data[0];
        Logger.Log($"Parsed fileTransferStatus: {fileTransferStatus}");
        FileTransferStatus = fileTransferStatus;
    }

    private void ParseFileTransferBlock(in byte[] data)
    {
        // FILL
    }

    private void ParseFileBytesTransferred(in byte[] data)
    {
        // FILL
    }

    public override void Reset()
    {
        base.Reset();

        _maxFileLength = null;
        _fileType = null;
        _fileLength = null;
        _fileChecksum = null;
        _fileTransferStatus = null;
        MTU = null;
        // FILL
    }
}
