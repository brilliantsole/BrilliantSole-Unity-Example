public partial class BS_Device
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseConnectionManager", BS_Logger.LogLevel.Log);

    private void Reset()
    {
        ConnectionStatus = BS_ConnectionStatus.NotConnected;
        DeviceInformation.Clear();
        // FILL
    }
}
