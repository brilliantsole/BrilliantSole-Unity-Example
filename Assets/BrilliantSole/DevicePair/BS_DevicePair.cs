public partial class BS_DevicePair
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePair");
    public static readonly BS_DevicePair Insoles;
    public static readonly BS_DevicePair Gloves;

    static BS_DevicePair()
    {
        Logger.Log("initializing Instance...");
        Insoles = new(BS_DevicePairType.Insoles);
        Gloves = new(BS_DevicePairType.Gloves);
        //BS_DeviceManager.OnDeviceConnected += (device) => { Instance.AddDevice(device); };
    }

    public readonly BS_DevicePairType Type;

    private BS_DevicePair(BS_DevicePairType type)
    {
        Type = type;
        Logger.Log($"initializing {Type} DevicePair...");
        SetupSensorDataManager();
    }

    public void Reset()
    {
        SensorDataManager.Reset();
    }

    public delegate void OnDeviceDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device
    );
}
