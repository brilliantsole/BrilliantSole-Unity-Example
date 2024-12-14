public abstract partial class BS_BaseClient
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClient", BS_Logger.LogLevel.Log);

    // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/c273625334a365a519b771b8fd2ea4b563514713/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_BaseClient.cpp#L29
    protected virtual void Reset()
    {
        _isScanning = false;
        _isScanningAvailable = false;

        _discoveredDevices.Clear();
        //_devices.Clear();

        foreach (var pair in _devices)
        {
            // FILL - set device connectionManager as not connected
        }
    }

    public virtual void Update() { }
}
