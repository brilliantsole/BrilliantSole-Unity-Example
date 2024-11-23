using UnityEngine;

public class BS_BleConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleConnectionManager", BS_Logger.LogLevel.Log);

    public override BS_ConnectionType Type => BS_ConnectionType.Ble;

    public BS_DiscoveredDevice DiscoveredDevice;
    public string Address => DiscoveredDevice.Id;

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue)
        {
            return;
        }
        // FILL
        Logger.Log($"Connecting to peripheral {Address}");
        BluetoothLEHardwareInterface.ConnectToPeripheral(Address, OnConnectToPeripheral, OnPeripheralService, OnPeripheralCharacteristic, OnPeripheralDisconnect);
    }

    private void OnConnectToPeripheral(string address)
    {
        Logger.Log($"Connected to peripheral {Address}");
        Status = BS_ConnectionStatus.Connected;
    }
    private void OnPeripheralService(string address, string serviceUuid)
    {
        Logger.Log($"Got Service {serviceUuid}");
    }
    private void OnPeripheralCharacteristic(string address, string serviceUuid, string characteristicUuid)
    {
        Logger.Log($"Got Characteristic {characteristicUuid} for service {serviceUuid}");
    }
    private void OnPeripheralDisconnect(string address)
    {
        Logger.Log("disconnected from peripheral");
        OnDisconnectPeripheral(address);
    }

    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue)
        {
            return;
        }
        // FILL - unsubscribe
        BluetoothLEHardwareInterface.DisconnectPeripheral(Address, OnDisconnectPeripheral);
    }

    private void OnDisconnectPeripheral(string address)
    {
        Logger.Log("Device Disconnected");
        Status = BS_ConnectionStatus.NotConnected;
    }

    public override void Update()
    {
        // FILL

    }
}
