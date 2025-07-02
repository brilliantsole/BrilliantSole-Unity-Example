using System.Collections.Generic;

public partial class BS_Device
{

    private readonly HashSet<BS_BaseManager> Managers = new();

    private void SetupManagers()
    {
        SetupBatteryManager();
        SetupInformationManager();
        SetupSensorConfigurationManager();
        SetupSensorDataManager();
        SetupVibrationManager();
        SetupFileTransferManager();
        SetupTfliteManager();
        // TODO
        // SetupWifiManager();
        // SetupCameraManager();
        // SetupMicrophoneManager();
        // SetupDisplayManager();
        foreach (var Manager in Managers) { Manager.SendTxMessages = SendTxMessages; }
    }

    private void ResetManagers() { foreach (var Manager in Managers) { Manager.Reset(); } }
}
