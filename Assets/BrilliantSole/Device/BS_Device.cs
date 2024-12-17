public partial class BS_Device
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_Device");

    private void Reset()
    {
        ResetBatteryLevel();
        ResetTxMessaging();
        ResetRxMessaging();
        DeviceInformation.Clear();
        ResetManagers();
        ConnectionStatus = BS_ConnectionStatus.NotConnected;
    }

    public BS_Device()
    {
        SetupManagers();
        BS_DeviceManager.OnDeviceCreated(this);
    }

    public BS_Device(string name, BS_DeviceType? deviceType) : this()
    {
        InformationManager.InitName(name);
        if (deviceType != null) { InformationManager.InitDeviceType((BS_DeviceType)deviceType); }
    }

    public BS_Device(BS_DiscoveredDevice discoveredDevice) : this(discoveredDevice.Name, discoveredDevice.DeviceType) { }
}
