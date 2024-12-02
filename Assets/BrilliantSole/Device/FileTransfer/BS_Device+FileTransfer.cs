using System;

public partial class BS_Device
{
    private readonly BS_FileTransferManager FileTransferManager = new();

    private void SetupFileTransferManager()
    {
        Managers.Add(FileTransferManager);

        // FILL
    }
}
