public abstract partial class BS_BaseClient
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClient", BS_Logger.LogLevel.Log);

    private void Reset()
    {
        _isScanning = false;
        _isScanningAvailable = false;

        _discoveredDevices.Clear();
        // FILL
        // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/c273625334a365a519b771b8fd2ea4b563514713/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_BaseClient.cpp#L29
    }
}
