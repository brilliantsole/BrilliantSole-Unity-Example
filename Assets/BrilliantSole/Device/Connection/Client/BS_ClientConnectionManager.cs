using System;
using System.Collections.Generic;
using System.Linq;
using static BS_ConnectionStatus;

public class BS_ClientConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ClientConnectionManager");

    public override BS_ConnectionType Type => BS_ConnectionType.Udp;

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }
        Client.SendConnectToDeviceMessage(bluetoothId);
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }
        Client.SendDisconnectFromDeviceMessage(bluetoothId);
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

    public override bool IsConnected => isConnected;

    public override void SendTxData(List<byte> data)
    {
        base.SendTxData(data);
        if (Client == null)
        {
            Logger.LogError("Client is not defined");
            return;
        }
        Client.SendDeviceMessages(bluetoothId, new() { BS_ConnectionMessageUtils.CreateMessage(BS_MetaConnectionMessageType.Tx.ToString(), data) });
        OnSendTxData?.Invoke(this);
    }

    public void OnDeviceEvent(byte deviceEventTypeByte, in byte[] deviceEventData)
    {
        if (deviceEventTypeByte >= BS_DeviceEventMessageUtils.EnumStrings.Count)
        {
            Logger.LogError($"invalid deviceEventTypeByte {deviceEventTypeByte}");
            return;
        }
        var deviceEventType = BS_DeviceEventMessageUtils.EnumStrings[deviceEventTypeByte];
        Logger.Log($"deviceEventType: {deviceEventType}");

        if (deviceEventType == BS_ConnectionEventType.IsConnected.ToString())
        {
            var isConnected = deviceEventData[0] == 1;
            Logger.Log($"Received IsConnected Message {isConnected}");
            SetIsConnected(isConnected);
        }
        else
        {
            Logger.Log($"miscellaneous deviceEventType {deviceEventType}");
            if (BS_TxRxMessageUtils.EnumStringMap.TryGetValue(deviceEventType, out byte txRxMessageType))
            {
                OnRxMessage?.Invoke(this, txRxMessageType, deviceEventData);
                OnRxMessages?.Invoke(this);
            }
            else
            {
                Logger.LogError($"uncaught deviceEventType {deviceEventType}");
            }
        }
    }

    private static readonly string[] RequiredDeviceInformationMessageTypes = new[] {
        BS_BatteryLevelMessageType.BatteryLevel.ToString(),

        BS_DeviceInformationType.ManufacturerName.ToString(),
        BS_DeviceInformationType.ModelNumber.ToString(),
        BS_DeviceInformationType.SerialNumber.ToString(),
        BS_DeviceInformationType.SoftwareRevision.ToString(),
        BS_DeviceInformationType.HardwareRevision.ToString(),
        BS_DeviceInformationType.FirmwareRevision.ToString(),
    };
    private static readonly List<BS_ConnectionMessage> RequiredDeviceInformationMessages = RequiredDeviceInformationMessageTypes.Select(messageType => BS_ConnectionMessageUtils.CreateMessage(messageType)).ToList();
    private void RequestDeviceInformation()
    {
        if (Client == null)
        {
            Logger.LogError("Client is not defined");
            return;
        }
        Logger.Log($"request device information");
        Client.SendDeviceMessages(bluetoothId, RequiredDeviceInformationMessages);
    }
}
