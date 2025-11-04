using System.Collections.Generic;

public partial class BS_DevicePair
{
    public delegate void OnDeviceFileTypesDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileType[] fileTypes
    );
    public delegate void OnDeviceMaxFileLengthDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        ushort maxFileLength
    );
    public delegate void OnDeviceFileTransferStatusDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileTransferStatus fileTransferStatus
    );
    public delegate void OnDeviceFileChecksumDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        uint fileChecksum
    );
    public delegate void OnDeviceFileLengthDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        uint fileLength
    );
    public delegate void OnDeviceFileTypeDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileType fileType
    );
    public delegate void OnDeviceFileTransferProgressDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileType fileType,
        BS_FileTransferDirection fileTransferDirection,
        float fileTransferProgress
    );
    public delegate void OnDeviceFileTransferCompleteDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileType fileType,
        BS_FileTransferDirection fileTransferDirection
    );
    public delegate void OnDeviceFileReceivedDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_FileType fileType,
        List<byte> fileData
    );

    public event OnDeviceFileTypesDelegate OnDeviceFileTypes;
    public event OnDeviceMaxFileLengthDelegate OnDeviceMaxFileLength;
    public event OnDeviceFileTransferStatusDelegate OnDeviceFileTransferStatus;
    public event OnDeviceFileChecksumDelegate OnDeviceFileChecksum;
    public event OnDeviceFileLengthDelegate OnDeviceFileLength;
    public event OnDeviceFileTypeDelegate OnDeviceFileType;
    public event OnDeviceFileTransferProgressDelegate OnDeviceFileTransferProgress;
    public event OnDeviceFileTransferCompleteDelegate OnDeviceFileTransferComplete;
    public event OnDeviceFileReceivedDelegate OnDeviceFileReceived;


    private void AddDeviceFileTransferListeners(BS_Device device)
    {
        device.OnFileTypes += onDeviceFileTypes;
        device.OnMaxFileLength += onDeviceMaxFileLength;
        device.OnFileTransferStatus += onDeviceFileTransferStatus;
        device.OnFileChecksum += onDeviceFileChecksum;
        device.OnFileLength += onDeviceFileLength;
        device.OnFileType += onDeviceFileType;
        device.OnFileTransferProgress += onDeviceFileTransferProgress;
        device.OnFileTransferComplete += onDeviceFileTransferComplete;
        device.OnFileReceived += onDeviceFileReceived;
    }
    private void RemoveDeviceFileTransferListeners(BS_Device device)
    {
        device.OnFileTypes -= onDeviceFileTypes;
        device.OnMaxFileLength -= onDeviceMaxFileLength;
        device.OnFileTransferStatus -= onDeviceFileTransferStatus;
        device.OnFileChecksum -= onDeviceFileChecksum;
        device.OnFileLength -= onDeviceFileLength;
        device.OnFileType -= onDeviceFileType;
        device.OnFileTransferProgress -= onDeviceFileTransferProgress;
        device.OnFileTransferComplete -= onDeviceFileTransferComplete;
        device.OnFileReceived -= onDeviceFileReceived;
    }


    private void onDeviceFileTypes(BS_Device device, BS_FileType[] fileTypes)
    {
        OnDeviceFileTypes?.Invoke(this, (BS_Side)device.Side, device, fileTypes);
    }
    private void onDeviceMaxFileLength(BS_Device device, ushort maxFileLength)
    {
        OnDeviceMaxFileLength?.Invoke(this, (BS_Side)device.Side, device, maxFileLength);
    }
    private void onDeviceFileTransferStatus(BS_Device device, BS_FileTransferStatus fileTransferStatus)
    {
        OnDeviceFileTransferStatus?.Invoke(this, (BS_Side)device.Side, device, fileTransferStatus);
    }
    private void onDeviceFileChecksum(BS_Device device, uint fileChecksum)
    {
        OnDeviceFileChecksum?.Invoke(this, (BS_Side)device.Side, device, fileChecksum);
    }
    private void onDeviceFileLength(BS_Device device, uint fileLength)
    {
        OnDeviceFileLength?.Invoke(this, (BS_Side)device.Side, device, fileLength);
    }
    private void onDeviceFileType(BS_Device device, BS_FileType fileType)
    {
        OnDeviceFileType?.Invoke(this, (BS_Side)device.Side, device, fileType);
    }
    private void onDeviceFileTransferProgress(BS_Device device, BS_FileType fileType, BS_FileTransferDirection fileTransferDirection, float progress)
    {
        OnDeviceFileTransferProgress?.Invoke(this, (BS_Side)device.Side, device, fileType, fileTransferDirection, progress);
    }
    private void onDeviceFileTransferComplete(BS_Device device, BS_FileType fileType, BS_FileTransferDirection fileTransferDirection)
    {
        OnDeviceFileTransferComplete?.Invoke(this, (BS_Side)device.Side, device, fileType, fileTransferDirection);
    }
    private void onDeviceFileReceived(BS_Device device, BS_FileType fileType, List<byte> fileReceived)
    {
        OnDeviceFileReceived?.Invoke(this, (BS_Side)device.Side, device, fileType, fileReceived);
    }
}
