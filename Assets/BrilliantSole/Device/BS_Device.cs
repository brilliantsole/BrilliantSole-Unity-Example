public partial class BS_Device
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseConnectionManager", BS_Logger.LogLevel.Log);

    private void Reset()
    {
        ConnectionStatus = BS_ConnectionStatus.NotConnected;

        _batteryLevel = 0;

        IsSendingTxData = false;
        PendingTxMessages.Clear();

        DeviceInformation.Clear();
        foreach (var BaseManager in Managers) { BaseManager.Reset(); }
    }

    public BS_Device()
    {
        Managers = new BS_BaseManager[] { InformationManager, SensorConfigurationManager, SensorDataManager, VibrationManager };
        SetupManagers();
    }
}
