using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_FileTransferManager FileTransferManager = new();

    public event Action<BS_Device, ushort> OnMaxFileLength;
    public event Action<BS_Device, BS_FileTransferStatus> OnFileTransferStatus;
    public event Action<BS_Device, uint> OnFileChecksum;
    public event Action<BS_Device, uint> OnFileLength;
    public event Action<BS_Device, BS_FileType> OnFileType;
    public event Action<BS_Device, BS_FileType, BS_FileTransferDirection, float> OnFileTransferProgress;
    public event Action<BS_Device, BS_FileType, BS_FileTransferDirection> OnFileTransferComplete;
    public event Action<BS_Device, BS_FileType, List<byte>> OnFileReceived;

    private void SetupFileTransferManager()
    {
        Managers.Add(FileTransferManager);

        OnMtu += (device, mtu) => { FileTransferManager.MTU = mtu; };

        FileTransferManager.OnMaxFileLength += fileLength => OnMaxFileLength?.Invoke(this, fileLength);
        FileTransferManager.OnFileTransferStatus += fileLength => OnFileTransferStatus?.Invoke(this, fileLength);
        FileTransferManager.OnFileChecksum += fileChecksum => OnFileChecksum?.Invoke(this, fileChecksum);
        FileTransferManager.OnFileLength += fileLength => OnFileLength?.Invoke(this, fileLength);
        FileTransferManager.OnFileType += fileType => OnFileType?.Invoke(this, fileType);
        FileTransferManager.OnFileTransferProgress += (fileType, fileTransferDirection, progress) => OnFileTransferProgress?.Invoke(this, fileType, fileTransferDirection, progress);
        FileTransferManager.OnFileTransferComplete += (fileType, fileTransferDirection) => OnFileTransferComplete?.Invoke(this, fileType, fileTransferDirection);
        FileTransferManager.OnFileReceived += (fileType, fileReceived) => OnFileReceived?.Invoke(this, fileType, fileReceived);
    }

    public void CancelFileTransfer()
    {
        FileTransferManager.CancelFileTransfer();
    }
    public BS_FileTransferStatus FileTransferStatus => FileTransferManager.FileTransferStatus;

    private void SendFile(BS_FileMetadata fileMetadata)
    {
        FileTransferManager.SendFile(fileMetadata);
    }
    public void ReceiveFile(BS_FileType fileType)
    {
        FileTransferManager.ReceiveFile(fileType);
    }
}
