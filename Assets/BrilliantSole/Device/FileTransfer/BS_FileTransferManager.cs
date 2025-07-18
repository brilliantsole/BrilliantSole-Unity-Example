using System;
using System.Collections.Generic;
using UnityEngine;
using static BS_FileTransferMessageType;
using static BS_FileTransferStatus;
using static BS_FileTransferCommand;
using System.Threading.Tasks;
using System.Linq;

public class BS_FileTransferManager : BS_BaseManager<BS_FileTransferMessageType>
{
    public static readonly BS_FileTransferMessageType[] RequiredMessageTypes = {
        GetFileTypes,
        GetMaxFileLength,
        GetFileTransferType,
        GetFileLength,
        GetFileChecksum,
        GetFileTransferStatus,
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileTransferManager");


    public event Action<BS_FileType[]> OnFileTypes;
    public event Action<ushort> OnMaxFileLength;
    public event Action<BS_FileTransferStatus> OnFileTransferStatus;
    public event Action<uint> OnFileChecksum;
    public event Action<uint> OnFileLength;
    public event Action<BS_FileType> OnFileType;
    public event Action<BS_FileType, BS_FileTransferDirection, float> OnFileTransferProgress;
    public event Action<BS_FileType, BS_FileTransferDirection> OnFileTransferComplete;
    public event Action<BS_FileType, List<byte>> OnFileReceived;

    private readonly List<byte> FileToReceive = new();
    private byte[] FileToSend;
    private List<byte> FileBlockToSend;

    private bool WaitingToSendMoreData = false;
    private uint BytesTransferred = 0;
    public ushort? MTU;

    public override void Reset()
    {
        base.Reset();

        _fileTypes = Array.Empty<BS_FileType>();
        _maxFileLength = null;
        _fileType = null;
        _fileLength = null;
        _fileChecksum = null;
        _fileTransferStatus = null;
        MTU = null;

        FileToReceive.Clear();
        FileToSend = null;
        FileBlockToSend = null;

        BytesTransferred = 0;
        WaitingToSendMoreData = false;
    }

    public override void OnRxMessage(BS_FileTransferMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetFileTypes:
                ParseFileTypes(data);
                break;
            case GetMaxFileLength:
                ParseMaxFileLength(data);
                break;
            case GetFileTransferType:
            case BS_FileTransferMessageType.SetFileTransferType:
                ParseFileTransferType(data);
                break;
            case GetFileLength:
            case BS_FileTransferMessageType.SetFileLength:
                ParseFileLength(data);
                break;
            case GetFileChecksum:
            case BS_FileTransferMessageType.SetFileChecksum:
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

    // FILE TYPES START
    [SerializeField]
    private BS_FileType[] _fileTypes = Array.Empty<BS_FileType>();
    public BS_FileType[] FileTypes
    {
        get => _fileTypes;
        private set
        {
            Logger.Log($"Updating FileTypes to {value}");
            _fileTypes = value;
            OnFileTypes?.Invoke(FileTypes);
        }
    }
    private void ParseFileTypes(in byte[] data)
    {
        BS_FileType[] fileTypes = new BS_FileType[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            fileTypes[i] = (BS_FileType)data[i];
        }
        Logger.Log($"Parsed fileTypes: {string.Join(", ", fileTypes)}");
        FileTypes = fileTypes;
    }
    // FILE TYPES END

    // MAX FILE LENGTH START
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
    private void ParseMaxFileLength(in byte[] data)
    {
        var maxFileLength = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"Parsed maxFileLength: {maxFileLength}");
        MaxFileLength = maxFileLength;
    }
    // MAX FILE LENGTH END

    // FILE TRANSFER TYPE START
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

    private void ParseFileTransferType(in byte[] data)
    {
        var fileType = (BS_FileType)data[0];
        Logger.Log($"Parsed fileType: {fileType}");
        FileType = fileType;
    }
    private void SetFileTransferType(BS_FileType newFileType, bool sendImmediately = true)
    {
        if (newFileType == FileType)
        {
            Logger.Log($"redundant fileType {newFileType}");
            return;
        }
        Logger.Log($"setting fileType to {newFileType}...");

        List<byte> data = new() { (byte)newFileType };
        BS_TxMessage[] Messages = { CreateMessage(BS_FileTransferMessageType.SetFileTransferType, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // FILE TRANSFER TYPE END

    // FILE LENGTH START
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

            if (FileTransferStatus == Receiving)
            {
                FileToReceive.Clear();
                FileToReceive.Capacity = (int)FileLength;
            }
        }
    }

    private void ParseFileLength(in byte[] data)
    {
        var fileLength = BS_ByteUtils.ParseNumber<uint>(data, isLittleEndian: true);
        Logger.Log($"Parsed fileLength: {fileLength}");
        FileLength = fileLength;
    }
    private void SetFileLength(uint newFileLength, bool sendImmediately = true)
    {
        if (newFileLength == FileLength)
        {
            Logger.Log($"redundant fileLength {newFileLength}");
            return;
        }
        Logger.Log($"setting fileLength to {newFileLength}...");

        BS_TxMessage[] Messages = { CreateMessage(BS_FileTransferMessageType.SetFileLength, BS_ByteUtils.ToByteArray(newFileLength, true)) };
        SendTxMessages(Messages, sendImmediately);
    }
    // FILE LENGTH END

    // FILE CHECKSUM START
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

    private void ParseFileChecksum(in byte[] data)
    {
        var fileChecksum = BS_ByteUtils.ParseNumber<uint>(data, isLittleEndian: true);
        Logger.Log($"Parsed fileChecksum: {fileChecksum}");
        FileChecksum = fileChecksum;
    }
    private void SetFileChecksum(uint newFileChecksum, bool sendImmediately = true)
    {
        if (newFileChecksum == FileChecksum)
        {
            Logger.Log($"redundant fileChecksum {newFileChecksum}");
            return;
        }
        Logger.Log($"setting fileChecksum to {newFileChecksum}...");

        BS_TxMessage[] Messages = { CreateMessage(BS_FileTransferMessageType.SetFileChecksum, BS_ByteUtils.ToByteArray(newFileChecksum, true)) };
        SendTxMessages(Messages, sendImmediately);
    }
    private uint GetCrc32(IEnumerable<byte> bytes)
    {
        var checksum = BS_CRC32.Compute(bytes);
        Logger.Log($"checksum: {checksum}");
        return checksum;
    }
    // FILE CHECKSUM END

    // FILE TRANSFER COMMAND START
    private void SetFileTransferCommand(BS_FileTransferCommand fileTransferCommand, bool sendImmediately = true)
    {
        Logger.Log($"setting fileTransferCommand {fileTransferCommand}...");

        List<byte> data = new() { (byte)fileTransferCommand };
        BS_TxMessage[] Messages = { CreateMessage(BS_FileTransferMessageType.SetFileTransferCommand, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // FILE TRANSFER COMMAND END

    // FILE TRANSFER STATUS START
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

            BytesTransferred = 0;

            if (FileTransferStatus == Sending)
            {
                Logger.Log($"Starting to send file...");
                SendFileBlock(false);
            }
        }
    }

    private void ParseFileTransferStatus(in byte[] data)
    {
        var fileTransferStatus = (BS_FileTransferStatus)data[0];
        Logger.Log($"Parsed fileTransferStatus: {fileTransferStatus}");
        FileTransferStatus = fileTransferStatus;
    }
    // FILE TRANSFER STATUS END

    // FILE BLOCK START
    public async Task<bool> SendFile(BS_FileMetadata fileMetadata)
    {
        if (!FileTypes.Contains(fileMetadata.FileType))
        {
            Logger.LogError($"cannot send file - fileType {fileMetadata.FileType} is not supported");
            return false;
        }
        if (FileTransferStatus != Idle)
        {
            Logger.LogWarning($"cannot send file - transferStatus is {FileTransferStatus}");
            return false;
        }

        var fileData = await fileMetadata.GetFileData();
        if (fileData == null)
        {
            Logger.LogError("failed to get filedata");
            return false;
        }
        Logger.Log($"sending {fileMetadata.FileType} file with {fileData.Length} bytes");

        var fileChecksum = GetCrc32(fileData);

        if (fileMetadata.FileType != FileType)
        {
            // different file types - sending
        }
        else if (fileData.Length != FileLength)
        {
            // different file lengths - sending
        }
        else if (FileChecksum != fileChecksum)
        {
            // different file checksums - sending
        }
        else
        {
            Logger.Log($"already sent file");
            return false;
        }

        FileToSend = fileData;

        SetFileTransferType(fileMetadata.FileType, false);
        SetFileLength((uint)FileToSend.Length, false);
        SetFileChecksum(fileChecksum, false);
        SetFileTransferCommand(Send);
        return true;
    }
    public void ReceiveFile(BS_FileType fileType)
    {
        if (FileTransferStatus != Idle)
        {
            Logger.Log($"cannot receive file - status ({FileTransferStatus}) isn't idle");
            return;
        }
        SetFileTransferType(fileType, false);
        SetFileTransferCommand(Receive);
    }

    private void SendFileBlock(bool sendImmediately)
    {
        if (FileTransferStatus != Sending)
        {
            Logger.LogError($"cannot send block - status ({FileTransferStatus}) is not {Sending}");
            return;
        }
        if (FileToSend.Length == 0)
        {
            Logger.LogError("FileToSend is Empty");
            return;
        }

        uint remainingBytes = (uint)(FileToSend.Length - BytesTransferred);
        Logger.Log($"remainingBytes: {remainingBytes}");

        var progress = BytesTransferred / (float)FileToSend.Length;
        Logger.Log($"progress: {progress}");
        OnFileTransferProgress?.Invoke(FileType, BS_FileTransferDirection.Sending, progress);

        if (remainingBytes == 0)
        {
            Logger.Log("file transfer complete");
            OnFileTransferComplete?.Invoke(FileType, BS_FileTransferDirection.Sending);
            WaitingToSendMoreData = false;
            return;
        }
        WaitingToSendMoreData = true;

        uint maxMessageLength = (uint)(MTU - 3 - 3);
        uint fileBlockLength = Math.Min(remainingBytes, maxMessageLength);
        Logger.Log($"maxMessageLength: {maxMessageLength}, fileBlockLength: {fileBlockLength}");

        FileBlockToSend = new List<byte>(new ArraySegment<byte>(FileToSend, (int)BytesTransferred, (int)fileBlockLength));
        BytesTransferred += (ushort)fileBlockLength;
        Logger.Log($"BytesTransferred: {BytesTransferred}");

        BS_TxMessage[] Messages = { CreateMessage(SetFileTransferBlock, FileBlockToSend) };
        SendTxMessages(Messages, sendImmediately);
    }
    public void CancelFileTransfer(bool sendImmediately = true)
    {
        if (FileTransferStatus == Idle)
        {
            Logger.Log("no need to cancel - already idle");
            return;
        }
        Logger.Log("Cancelling file transfer...");
        SetFileTransferCommand(Cancel, sendImmediately);
    }
    private void ParseFileTransferBlock(in byte[] data)
    {
        if (FileTransferStatus != Receiving)
        {
            Logger.LogError($"cannot send block - status ({FileTransferStatus}) is not {Receiving}");
            return;
        }

        ushort fileBlockLength = (ushort)data.Length;
        Logger.Log($"received fileBlock of {fileBlockLength} bytes");

        uint currentFileLength = (ushort)FileToReceive.Count;
        uint newFileLength = currentFileLength + fileBlockLength;
        Logger.Log($"updating FileToReceive from {currentFileLength} to {newFileLength} bytes...");

        if (newFileLength > FileLength)
        {
            Logger.LogError($"newFileLength {newFileLength} is larger than expected FileLength {FileLength} - cancelling");
            CancelFileTransfer();
            return;
        }

        FileToReceive.AddRange(data);
        currentFileLength = (ushort)FileToReceive.Count;
        var progress = FileToReceive.Count / (float)FileLength;
        Logger.Log($"FileToReceive length: {FileToReceive.Count}/{FileLength} ({progress}%)");
        OnFileTransferProgress?.Invoke(FileType, BS_FileTransferDirection.Receiving, progress);

        if (currentFileLength == FileLength)
        {
            Logger.Log("Finished receiving file");
            var receivedFileChecksum = GetCrc32(FileToReceive);
            if (receivedFileChecksum != FileChecksum)
            {
                Logger.LogError($"filechecksums don't match - expected {FileChecksum}, got {receivedFileChecksum}");
                return;
            }
            Logger.Log($"File Checksums match {FileChecksum}");
            OnFileTransferComplete(FileType, BS_FileTransferDirection.Receiving);
            OnFileReceived?.Invoke(FileType, FileToReceive);
        }
        else
        {
            var messageData = BS_ByteUtils.ToByteArray(currentFileLength, true);
            BS_TxMessage[] Messages = { CreateMessage(FileBytesTransferred, messageData) };
            SendTxMessages(Messages, false);
        }
    }

    private void ParseFileBytesTransferred(in byte[] data)
    {
        if (FileTransferStatus != Sending)
        {
            Logger.LogError($"Currently not sending file");
            return;
        }
        if (!WaitingToSendMoreData)
        {
            Logger.LogError($"Not waiting to send more data");
            return;
        }

        var currentBytesTransferred = BS_ByteUtils.ParseNumber<ushort>(data, 0, true);
        Logger.Log($"currentBytesTransferred: {currentBytesTransferred}");

        if (BytesTransferred != currentBytesTransferred)
        {
            Logger.LogError($"BytesTransferred not equal - got {currentBytesTransferred}, expected {currentBytesTransferred}");
            CancelFileTransfer();
            return;
        }

        Logger.Log("Sending next file block...");
        SendFileBlock(false);
    }

    // FILE BLOCK END
}
