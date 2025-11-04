using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BS_Device
{
    private readonly BS_FileTransferManager FileTransferManager = new();

    public delegate void OnFileTypesDelegate(BS_Device device, BS_FileType[] fileTypes);
    public delegate void OnMaxFileLengthDelegate(BS_Device device, ushort maxFileLength);
    public delegate void OnFileTransferStatusDelegate(BS_Device device, BS_FileTransferStatus fileTransferStatus);
    public delegate void OnFileChecksumDelegate(BS_Device device, uint fileChecksum);
    public delegate void OnFileLengthDelegate(BS_Device device, uint fileLength);
    public delegate void OnFileTypeDelegate(BS_Device device, BS_FileType fileType);
    public delegate void OnFileTransferProgressDelegate(BS_Device device, BS_FileType fileType, BS_FileTransferDirection fileTransferDirection, float fileTransferProgress);
    public delegate void OnFileTransferCompleteDelegate(BS_Device device, BS_FileType fileType, BS_FileTransferDirection fileTransferDirection);
    public delegate void OnFileReceivedDelegate(BS_Device device, BS_FileType fileType, List<byte> fileData);

    public event OnFileTypesDelegate OnFileTypes;
    public event OnMaxFileLengthDelegate OnMaxFileLength;
    public event OnFileTransferStatusDelegate OnFileTransferStatus;
    public event OnFileChecksumDelegate OnFileChecksum;
    public event OnFileLengthDelegate OnFileLength;
    public event OnFileTypeDelegate OnFileType;
    public event OnFileTransferProgressDelegate OnFileTransferProgress;
    public event OnFileTransferCompleteDelegate OnFileTransferComplete;
    public event OnFileReceivedDelegate OnFileReceived;


    private void SetupFileTransferManager()
    {
        Managers.Add(FileTransferManager);

        OnMtu += (device, mtu) => { FileTransferManager.MTU = mtu; };

        FileTransferManager.OnFileTypes += fileTypes => OnFileTypes?.Invoke(this, fileTypes);
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

    private async Task<bool> SendFile(BS_FileMetadata fileMetadata)
    {
        return await FileTransferManager.SendFile(fileMetadata);
    }
    public void ReceiveFile(BS_FileType fileType)
    {
        FileTransferManager.ReceiveFile(fileType);
    }
}
