public partial class BS_DevicePair
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePair");
    public static readonly BS_DevicePair Instance;
    public bool IsInstance => Instance == this;

    static BS_DevicePair()
    {
        Logger.Log("initializing Instance...");
        Instance = new();
        //BS_DeviceManager.OnDeviceConnected += (device) => { Instance.AddDevice(device); };
    }

    public BS_DevicePair()
    {
        Logger.Log("initializing DevicePair...");
        SetupSensorDataManager();
    }

    public void Reset()
    {
        SensorDataManager.Reset();
    }
}
