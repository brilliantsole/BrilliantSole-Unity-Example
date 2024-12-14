using System;
using System.Collections.Generic;
using System.Linq;
using static BS_ServerMessageType;

public partial class BS_BaseClient
{
    protected void OnData(in byte[] data)
    {
        Logger.Log($"received {data.Length} bytes...");
        ParseServerData(data);
    }
    private void ParseServerData(in byte[] data)
    {
        Logger.Log($"parsing {data.Length} bytes...");
        BS_ParseUtils.ParseMessages(data, OnParsedServerMessage);
    }
    private void OnParsedServerMessage(byte serverMessageTypeByte, byte[] messageData)
    {
        if (Enum.IsDefined(typeof(BS_ServerMessageType), serverMessageTypeByte))
        {
            Logger.LogError($"invalid serverMessageTypeByte {serverMessageTypeByte}");
            return;
        }
        var serverMessageType = (BS_ServerMessageType)serverMessageTypeByte;
        Logger.Log($"serverMessageType: {serverMessageType}");

        switch (serverMessageType)
        {
            case BS_ServerMessageType.IsScanningAvailable:
                ParseIsScanningAvailable(messageData);
                break;
            case BS_ServerMessageType.IsScanning:
                ParseIsScanning(messageData);
                break;
            case DiscoveredDevice:
                ParseDiscoveredDevice(messageData);
                break;
            case ExpiredDiscoveredDevice:
                ParseExpiredDiscoveredDevice(messageData);
                break;
            case ConnectedDevices:
                ParseConnectedDevices(messageData);
                break;
            case DeviceMessage:
                ParseDeviceMessage(messageData);
                break;
            default:
                Logger.LogError($"Uncaught serverMessageType {serverMessageType}");
                break;
        }
    }

    private void SendMessages(List<BS_ServerMessage> serverMessages, bool sendImmediately = true)
    {
        List<byte> MessageData = new();
        for (int i = 0; i < serverMessages.Count; i++) { serverMessages[i].AppendTo(MessageData); }

        if (MessageData.Count == 0)
        {
            Logger.Log("MessageData is empty - not sending");
            return;
        }
        Logger.Log($"sending {MessageData.Count} bytes...");

        SendMessageData(MessageData, sendImmediately);
    }

    private void SendRequiredMessages() { SendMessages(RequiredMessages); }
    private static readonly List<BS_ServerMessageType> RequiredMessageTypes = new(){
        BS_ServerMessageType.IsScanningAvailable,
        BS_ServerMessageType.DiscoveredDevices,
        ConnectedDevices
    };
    private static readonly List<BS_ServerMessage> RequiredMessages = RequiredMessageTypes.Select(messageType => new BS_ServerMessage(messageType)).ToList();

    protected virtual void SendMessageData(List<byte> data, bool sendImmediately = true) { Logger.Log($"sending {data.Count} bytes..."); }
}
