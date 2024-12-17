using System.Collections.Generic;

public partial class BS_BaseClient
{
    public void SendConnectToDeviceMessage(string bluetoothId, bool sendImmediately = true)
    {
        Logger.Log($"requesting connection to {bluetoothId}");
        List<byte> serverMessage = new(BS_StringUtils.ToBytes(bluetoothId, true));
        SendMessages(new() { new(BS_ServerMessageType.ConnectToDevice, serverMessage) }, sendImmediately);
    }
    public void SendDisconnectFromDeviceMessage(string bluetoothId, bool sendImmediately = true)
    {
        Logger.Log($"requesting disconnection from {bluetoothId}");
        List<byte> serverMessage = new(BS_StringUtils.ToBytes(bluetoothId, true));
        SendMessages(new() { new(BS_ServerMessageType.DisconnectFromDevice, serverMessage) }, sendImmediately);
    }
    public void SendDeviceMessages(string bluetoothId, List<BS_ConnectionMessage> messages, bool sendImmediately = true)
    {
        Logger.Log($"sending {messages.Count} messages to {bluetoothId}");
        List<byte> serverMessage = new();
        serverMessage.AddRange(BS_StringUtils.ToBytes(bluetoothId, true));
        for (int i = 0; i < messages.Count; i++)
        {
            Logger.Log($"Appending {messages[i].Type} ({messages[i].DataLength()} bytes) to message");
            messages[i].AppendTo(serverMessage);
        }
        SendMessages(new() { new(BS_ServerMessageType.DeviceMessage, serverMessage) }, sendImmediately);
    }

    private void ParseDeviceMessage(in byte[] data)
    {
        Logger.Log($"parsing device message ({data.Length} bytes)...");

        var offset = 0;
        var bluetoothId = BS_StringUtils.GetString(data, true);
        offset += 1 + bluetoothId.Length;

        Logger.Log($"received device message from {bluetoothId}");
        if (_allDevices.TryGetValue(bluetoothId, out BS_Device device))
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
