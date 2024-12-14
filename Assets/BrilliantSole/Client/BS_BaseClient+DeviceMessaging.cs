public partial class BS_BaseClient
{
    private void ParseDeviceMessage(in byte[] data)
    {
        Logger.Log($"parsing device message ({data.Length} bytes)...");

        var offset = 0;
        var bluetoothId = BS_StringUtils.GetString(data, true);
        offset += 1 + bluetoothId.Length;

        Logger.Log($"received device message from {bluetoothId}");
        if (_devices.TryGetValue(bluetoothId, out BS_Device device))
        {
            var messageDataLength = data.Length - offset;
            var messageData = BS_ParseUtils.GetSubarray(data, (ushort)offset, (ushort)messageDataLength);

            Logger.Log($"parsing {messageDataLength} bytes for device...");
            // FILL - send data to connectionManager
            // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/c273625334a365a519b771b8fd2ea4b563514713/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_BaseClient.cpp#L485
        }
        else
        {
            Logger.LogError($"no device found with bluetoothId {bluetoothId}");
        }
    }
}
