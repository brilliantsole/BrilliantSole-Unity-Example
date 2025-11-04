using System.Collections.Generic;
using System.Linq;
using static BS_ConnectionStatus;
using static BS_DeviceInformationType;

public class BS_ClientConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ClientConnectionManager");

    public override BS_ConnectionType Type => BS_ConnectionType.Udp;

    public override bool IsAvailable => Client.IsConnected;

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
        // if (newIsConnected == isConnected)
        // {
        //     Logger.Log($"redundant IsConected assignment {newIsConnected}");
        //     return;
        // }
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
        Logger.Log($"deviceEventType {deviceEventTypeByte}: {deviceEventType} ({deviceEventData.Length} bytes)");

        if (deviceEventType == BS_ConnectionEventType.IsConnected.ToString())
        {
            var isConnected = deviceEventData[0] == 1;
            Logger.Log($"Received IsConnected message {isConnected}");
            SetIsConnected(isConnected);
        }
        else if (deviceEventType == BS_MetaConnectionMessageType.Rx.ToString())
        {
            Logger.Log("received RX message");
            ParseRxData(deviceEventData);
        }
        else if (deviceEventType == BS_BatteryLevelMessageType.BatteryLevel.ToString())
        {
            Logger.Log("Received Battery Level message");
            var batteryLevel = deviceEventData[0];
            OnBatteryLevel?.Invoke(this, batteryLevel);
        }
        else if (deviceEventType == ManufacturerName.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, ManufacturerName, deviceEventData);
        }
        else if (deviceEventType == ModelNumber.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, ModelNumber, deviceEventData);
        }
        else if (deviceEventType == SoftwareRevision.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, SoftwareRevision, deviceEventData);
        }
        else if (deviceEventType == HardwareRevision.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, HardwareRevision, deviceEventData);
        }
        else if (deviceEventType == FirmwareRevision.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, FirmwareRevision, deviceEventData);
        }
        else if (deviceEventType == SerialNumber.ToString())
        {
            OnDeviceInformationValue?.Invoke(this, SerialNumber, deviceEventData);
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

        ManufacturerName.ToString(),
        ModelNumber.ToString(),
        SerialNumber.ToString(),
        SoftwareRevision.ToString(),
        HardwareRevision.ToString(),
        FirmwareRevision.ToString(),
    };
    private static readonly List<BS_ConnectionMessage> RequiredDeviceInformationMessages = RequiredDeviceInformationMessageTypes.Select(messageType => BS_ConnectionMessageUtils.CreateMessage(messageType)).ToList();
    private void RequestDeviceInformation()
    {
        if (Client == null)
        {
            Logger.LogError("Client is not defined");
            return;
        }
        Logger.Log("request device information");
        Client.SendDeviceMessages(bluetoothId, RequiredDeviceInformationMessages);
    }
}
