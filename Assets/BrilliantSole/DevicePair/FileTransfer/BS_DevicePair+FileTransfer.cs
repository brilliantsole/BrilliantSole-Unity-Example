using System;
using System.Collections.Generic;

public partial class BS_DevicePair
{
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileType[]> OnDeviceFileTypes;
    public event Action<BS_DevicePair, BS_Side, BS_Device, ushort> OnDeviceMaxFileLength;
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileTransferStatus> OnDeviceFileTransferStatus;
    public event Action<BS_DevicePair, BS_Side, BS_Device, uint> OnDeviceFileChecksum;
    public event Action<BS_DevicePair, BS_Side, BS_Device, uint> OnDeviceFileLength;
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileType> OnDeviceFileType;
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileType, BS_FileTransferDirection, float> OnDeviceFileTransferProgress;
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileType, BS_FileTransferDirection> OnDeviceFileTransferComplete;
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_FileType, List<byte>> OnDeviceFileReceived;

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
