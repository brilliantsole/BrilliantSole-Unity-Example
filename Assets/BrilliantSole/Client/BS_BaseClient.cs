using System.Linq;

public abstract partial class BS_BaseClient : IBS_Scanner
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClient");

    protected virtual void Reset()
    {
        Logger.Log("Resetting");

        IsScanning = false;
        IsScanningAvailable = false;

        _discoveredDevices.Clear();
        //_devices.Clear();

        foreach (var device in _devices.Values.ToList())
        {
            if (device.ConnectionManager is BS_ClientConnectionManager connectionManager)
            {
                device._SetConnectionStatus(BS_ConnectionStatus.NotConnected);
                connectionManager.SetIsConnected(false);
                BS_DeviceManager._OnIsDeviceConnected(device, false);
            }
            else
            {
                Logger.LogError("failed to cast ConnectionManager to BS_ClientConnectionManager");
            }
        }
    }

    public virtual void Update() { }
}
