public partial class BS_Device
{
    private readonly BS_FileTransferManager FileTransferManager = new();

    private void SetupFileTransferManager()
    {
        Managers.Add(FileTransferManager);

        OnMtu += (BS_Device device, ushort mtu) => { FileTransferManager.MTU = mtu; };
    }
}
