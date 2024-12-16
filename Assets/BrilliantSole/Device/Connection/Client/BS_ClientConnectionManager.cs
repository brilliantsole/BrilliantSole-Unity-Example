using System;
using System.Collections.Generic;
using static BS_ConnectionStatus;

public class BS_ClientConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ClientConnectionManager");

    public override BS_ConnectionType Type => BS_ConnectionType.Udp;

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }
        // FILL
        // Client.SendConnectToDeviceMessage();
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }
        // FILL
        // Client.SendDisconnectFromDeviceMessage();
    }

    public BS_BaseClient Client;
    public string bluetoothId;

    private bool isConnected = false;
    public void SetIsConnected(bool newIsConnected)
    {
        if (newIsConnected == isConnected)
        {
            Logger.Log($"redundant IsConected assignment {newIsConnected}");
            return;
        }
        isConnected = newIsConnected;
        Logger.Log($"updated IsConnected to {IsConnected}");
        Status = IsConnected ? Connected : NotConnected;

        if (IsConnected)
        {
            RequestDeviceInformation();
        }
    }

    private void RequestDeviceInformation()
    {
        if (Client == null)
        {
            Logger.LogError("Client is not defined");
            return;
        }
        Logger.Log($"request device information");
        // FILL
        // Client.SendDeviceMessages();
    }

    public override bool IsConnected => isConnected;

    public override void SendTxData(List<byte> Data)
    {
        base.SendTxData(Data);
        if (Client == null)
        {
            Logger.LogError("Client is not defined");
            return;
        }
        // FILL
        // Client.SendDeviceMessages();
        OnSendTxData?.Invoke(this);
    }

    public void OnDeviceEvent(byte deviceEventByte, in byte[] deviceEventData)
    {
        if (!Enum.IsDefined(typeof(BS_ServerMessageType), deviceEventByte))
        {
            Logger.LogError($"invalid deviceEventByte {deviceEventByte}");
            return;
        }
        // FIX
        // var serverMessageType = (BS_ServerMessageType)serverMessageTypeByte;
        // Logger.Log($"serverMessageType: {serverMessageType}");

        // switch (serverMessageType)
        // {
        //     case BS_ServerMessageType.IsScanningAvailable:
        //         ParseIsScanningAvailable(messageData);
        //         break;
        //     case BS_ServerMessageType.IsScanning:
        //         ParseIsScanning(messageData);
        //         break;
        //     case DiscoveredDevice:
        //         ParseDiscoveredDevice(messageData);
        //         break;
        //     case ExpiredDiscoveredDevice:
        //         ParseExpiredDiscoveredDevice(messageData);
        //         break;
        //     case ConnectedDevices:
        //         ParseConnectedDevices(messageData);
        //         break;
        //     case DeviceMessage:
        //         ParseDeviceMessage(messageData);
        //         break;
        //     default:
        //         Logger.LogError($"Uncaught serverMessageType {serverMessageType}");
        //         break;
        // }
    }
}
