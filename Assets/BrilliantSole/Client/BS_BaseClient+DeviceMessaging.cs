public partial class BS_BaseClient
{
    // FILL

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
            if (device.ConnectionManager is BS_ClientConnectionManager connectionManager)
            {
                BS_ParseUtils.ParseMessages(messageData, (deviceEventByte, deviceEventData) => connectionManager.OnDeviceEvent(deviceEventByte, deviceEventData));
            }
            else
            {
                Logger.LogError($"failed to cast connectionManager as BS_ClientConnectionManager");
            }
        }
        else
        {
            Logger.LogError($"no device found with bluetoothId {bluetoothId}");
        }
    }
}
