using UnityEngine.InputSystem;

public partial class BS_DevicePair
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePair", BS_Logger.LogLevel.Warn);
    public static readonly BS_DevicePair Instance;
    public bool IsInstance => Instance == this;

    static BS_DevicePair()
    {
        Instance = new();
        BS_DeviceManager.OnDeviceConnected += (BS_Device device) => Instance.AddDevice(device);
    }

    public BS_DevicePair()
    {
        SetupSensorDataManager();
    }

    public void Reset()
    {
        SensorDataManager.Reset();
    }
}
