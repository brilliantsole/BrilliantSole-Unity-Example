using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BS_ConnectionStatus;

#nullable enable
public class BS_BleConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleConnectionManager");

    public override BS_ConnectionType Type => BS_ConnectionType.Ble;

    public BS_DiscoveredDevice? DiscoveredDevice;
    public string Address => DiscoveredDevice?.Id ?? "";
    public override string? Name => DiscoveredDevice?.Name;
    public override BS_DeviceType? DeviceType => DiscoveredDevice?.DeviceType;

    private void Reset()
    {
        ResetUuids();
        Stage = BS_BleConnectionStage.None;
    }

    private readonly HashSet<string> FoundServiceUuids = new();
    private string GetFoundServiceUuid(string serviceUuid)
    {
        foreach (var _serviceUuid in FoundServiceUuids)
        {
            if (BS_BleUtils.AreUuidsEqual(serviceUuid, _serviceUuid))
            {
                return _serviceUuid;
            }
        }
        return serviceUuid;
    }
    private readonly HashSet<string> FoundCharacteristicUuids = new();
    private string GetFoundCharacteristicUuid(string characteristicUuid)
    {
        foreach (var _characteristicUuid in FoundCharacteristicUuids)
        {
            if (BS_BleUtils.AreUuidsEqual(characteristicUuid, _characteristicUuid))
            {
                return _characteristicUuid;
            }
        }
        return characteristicUuid;
    }
    private void ResetUuids()
    {
        FoundServiceUuids.Clear();
        FoundCharacteristicUuids.Clear();
        ReadCharacteristicUuids.Clear();
        SubscribedCharacteristicUuids.Clear();
    }

    private bool CheckFoundAllServiceUuids()
    {
        bool FoundAllServiceUuids = true;
        foreach (var serviceUuid in BS_BleUtils.AllServiceUuids)
        {
            if (!FoundServiceUuids.Any((foundServiceId) => BS_BleUtils.AreUuidsEqual(foundServiceId, serviceUuid)))
            {
                FoundAllServiceUuids = false;
                Logger.Log($"Didn't find serviceUuid {serviceUuid}");
                break;
            }
        }
        return FoundAllServiceUuids;
    }
    private bool CheckFoundAllCharacteristicUuids()
    {
        bool FoundAllCharacteristicUuids = true;
        foreach (var characteristicUuid in BS_BleUtils.AllCharacteristicUuids)
        {
            if (!FoundCharacteristicUuids.Any((foundCharacteristicId) => BS_BleUtils.AreUuidsEqual(foundCharacteristicId, characteristicUuid)))
            {
                FoundAllCharacteristicUuids = false;
                Logger.Log($"Didn't find characteristicUuid {characteristicUuid}");
                break;
            }
        }
        return FoundAllCharacteristicUuids;
    }

    private bool DidFindAllUuids => CheckFoundAllServiceUuids() && CheckFoundAllCharacteristicUuids();

    private void CheckDidFindAllUuids()
    {
        if (Stage == BS_BleConnectionStage.WaitForUuids && DidFindAllUuids)
        {
            Logger.Log("Got all Uuids");
            Stage = BS_BleConnectionStage.RequestingMtu;
        }
    }

    [SerializeField]
    private float _timeout = 0f;
    private void UpdateTimeout()
    {
        _timeout = _Stage switch
        {
            BS_BleConnectionStage.Connecting => 0.1f,
            BS_BleConnectionStage.RequestingMtu => 0.1f,
            BS_BleConnectionStage.SubscribingToCharacteristics => 0.1f,
            BS_BleConnectionStage.ReadingCharacteristics => 0.1f,
            BS_BleConnectionStage.WritingTxCharacteristic => 0.01f,
            _ => 0f,
        };
        Logger.Log($"timeout: {_timeout}");
    }

    [SerializeField]
    private BS_BleConnectionStage _Stage = BS_BleConnectionStage.None;
    public BS_BleConnectionStage Stage
    {
        get => _Stage;
        private set
        {
            if (_Stage == value) { return; }
            Logger.Log($"Updating BS_BleConnectionStage to {value}");
            _Stage = value;
            UpdateTimeout();
        }
    }

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }
        Stage = BS_BleConnectionStage.Connecting;
    }
    private void ConnectToPeripheral()
    {
        Logger.Log($"Connecting to peripheral \"{Name}\"");
        BluetoothLEHardwareInterface.ConnectToPeripheral(Address, OnConnectToPeripheral, OnPeripheralService, OnPeripheralCharacteristic, OnPeripheralDisconnect);
    }

    private void OnConnectToPeripheral(string address)
    {
        if (address != Address) { return; }
        Logger.Log($"Connected to peripheral {Address}");
        Stage = BS_BleConnectionStage.WaitForUuids;
    }
    private void OnPeripheralService(string address, string serviceUuid)
    {
        if (address != Address) { return; }
        Logger.Log($"Got Service {serviceUuid}");
        FoundServiceUuids.Add(serviceUuid);
        CheckDidFindAllUuids();
    }
    private void OnPeripheralCharacteristic(string address, string serviceUuid, string characteristicUuid)
    {
        if (address != Address) { return; }
        Logger.Log($"Got Characteristic {characteristicUuid} for service {serviceUuid}");
        FoundCharacteristicUuids.Add(characteristicUuid);
        FoundServiceUuids.Add(serviceUuid);
        CheckDidFindAllUuids();
    }
    private void OnPeripheralDisconnect(string address)
    {
        if (address != Address) { return; }
        Logger.Log("disconnected from peripheral");
        OnDisconnectPeripheral(address);
    }

    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }
        Stage = BS_BleConnectionStage.UnsubscribingFromCharacteristics;
        DisconnectPeripheral();
    }
    private void DisconnectPeripheral()
    {
        Logger.Log($"Disconnecting from peripheral \"{Name}\"");
        BluetoothLEHardwareInterface.DisconnectPeripheral(Address, OnDisconnectPeripheral);
    }

    private void OnDisconnectPeripheral(string address)
    {
        if (address != Address) { return; }
        Logger.Log($"Disconnected from \"{Name}\"");
        Status = NotConnected;
        Reset();
    }

    private void RequestMtu()
    {
        Logger.Log($"Requesting Mtu from \"{Name}\"");
        BluetoothLEHardwareInterface.RequestMtu(Address, 512, OnPeripheralMtu);
    }

    private void OnPeripheralMtu(string address, int mtu)
    {
        if (address != Address) { return; }
        Logger.Log($"Updated Mtu of \"{Name}\" to {mtu}");
        Stage = BS_BleConnectionStage.ReadingCharacteristics;
    }

    private readonly HashSet<string> ReadCharacteristicUuids = new();
    private string? GetNextCharacteristicUuidToRead()
    {
        string? nextCharacteristicUuidToRead = null;
        foreach (var characteristicUuid in BS_BleUtils.ReadableCharacteristicUuids)
        {
            if (!ReadCharacteristicUuids.Any((readCharacteristicUuid) => BS_BleUtils.AreUuidsEqual(readCharacteristicUuid, characteristicUuid)))
            {
                nextCharacteristicUuidToRead = characteristicUuid;
                break;
            }
        }
        return nextCharacteristicUuidToRead;
    }

    private void ReadCharacteristics()
    {
        var characteristicUuidToRead = GetNextCharacteristicUuidToRead();
        if (characteristicUuidToRead == null)
        {
            Logger.Log("Read all Characteristics");
            Stage = BS_BleConnectionStage.SubscribingToCharacteristics;
            return;
        }
        Logger.Log($"nextCharacteristicUuidToRead {characteristicUuidToRead}");

        var serviceUuid = BS_BleUtils.GetServiceUuid(characteristicUuidToRead);
        if (serviceUuid == null)
        {
            Logger.LogError($"Unable to find serviceUuid for characteristicUuidToRead {characteristicUuidToRead}");
            return;
        }
#if !UNITY_IOS
        serviceUuid = GetFoundServiceUuid(serviceUuid);
        characteristicUuidToRead = GetFoundCharacteristicUuid(characteristicUuidToRead);
#endif
        Logger.Log($"reading characteristicUuidToRead {characteristicUuidToRead} of serviceUuid {serviceUuid}...");
        BluetoothLEHardwareInterface.ReadCharacteristic(Address, serviceUuid, characteristicUuidToRead, OnCharacteristicRead);
    }
    private void OnCharacteristicRead(string characteristicUuid, byte[] data)
    {
        Logger.Log($"Read {data.Length} data from characteristicUuid {characteristicUuid} for \"{Name}\"");
        OnCharacteristicValue(characteristicUuid, data);
        ReadCharacteristicUuids.Add(characteristicUuid);
        ReadCharacteristics();
    }

    private void OnCharacteristicWrite(string characteristicUuid)
    {
        Logger.Log($"Wrote to characteristicUuid {characteristicUuid} for \"{Name}\"");
        Stage = BS_BleConnectionStage.None;
        OnSendTxData?.Invoke(this);
    }

    private readonly HashSet<string> SubscribedCharacteristicUuids = new();
    private string? GetNextCharacteristicUuidToSubscribe()
    {
        string? nextCharacteristicUuidToSubscribe = null;
        foreach (var characteristicUuid in BS_BleUtils.NotifiableCharacteristicUuids)
        {
            if (!SubscribedCharacteristicUuids.Any((subscribedCharacteristicUuid) => BS_BleUtils.AreUuidsEqual(subscribedCharacteristicUuid, characteristicUuid)))
            {
                nextCharacteristicUuidToSubscribe = characteristicUuid;
                break;
            }
        }
        return nextCharacteristicUuidToSubscribe;
    }
    private void SubscribeToCharacteristics()
    {
        var characteristicUuidToSubscribe = GetNextCharacteristicUuidToSubscribe();
        if (characteristicUuidToSubscribe == null)
        {
            Logger.Log("Subscribed to all Characteristics");
            Stage = BS_BleConnectionStage.None;
            Status = Connected;
            return;
        }
        Logger.Log($"nextCharacteristicUuidToSubscribe {characteristicUuidToSubscribe}");

        var serviceUuid = BS_BleUtils.GetServiceUuid(characteristicUuidToSubscribe);
        if (serviceUuid == null)
        {
            Logger.LogError($"Unable to find serviceUuid for characteristicUuidToSubscribe {characteristicUuidToSubscribe}");
            return;
        }
        Logger.Log($"subscribing to characteristicUuidToSubscribe {characteristicUuidToSubscribe} of serviceUuid {serviceUuid}...");
        BluetoothLEHardwareInterface.SubscribeCharacteristic(Address, serviceUuid, characteristicUuidToSubscribe, OnCharacteristicSubscribed, OnCharacteristicNotify);
    }
    private void OnCharacteristicNotify(string characteristicUuid, byte[] data)
    {
        Logger.Log($"Was notified {data.Length} data from characteristicUuid {characteristicUuid} for \"{Name}\"");
        OnCharacteristicValue(characteristicUuid, data);
    }
    private void OnCharacteristicSubscribed(string characteristicUuid)
    {
        Logger.Log($"Subscribed to characteristicUuid {characteristicUuid} for \"{Name}\"");
        SubscribedCharacteristicUuids.Add(characteristicUuid);
        SubscribeToCharacteristics();
    }
    private string? GetNextCharacteristicUuidToUnsubscribe()
    {
        string? nextCharacteristicUuidToUnsubscribe = null;
        if (SubscribedCharacteristicUuids.Count > 0)
        {
            foreach (var characteristicUuidToUnsubscribe in SubscribedCharacteristicUuids)
            {
                nextCharacteristicUuidToUnsubscribe = characteristicUuidToUnsubscribe;
                break;
            }
        }
        return nextCharacteristicUuidToUnsubscribe;
    }
    private void UnsubscribeToCharacteristics()
    {
        string? characteristicUuidToUnsubscribe = GetNextCharacteristicUuidToUnsubscribe();
        if (characteristicUuidToUnsubscribe == null)
        {
            Logger.Log("Unsubscribed from all Characteristics");
            Stage = BS_BleConnectionStage.Disconnecting;
            return;
        }
        Logger.Log($"nextCharacteristicUuidToUnsubscribe {characteristicUuidToUnsubscribe}");

        string? serviceUuid = BS_BleUtils.GetServiceUuid(characteristicUuidToUnsubscribe);
        if (serviceUuid == null)
        {
            Logger.LogError($"Unable to find serviceUuid for characteristicUuidToUnsubscribe {characteristicUuidToUnsubscribe}");
            return;
        }
        Logger.Log($"unsubscribing from characteristicUuidToUnsubscribe {characteristicUuidToUnsubscribe} of serviceUuid {serviceUuid}...");
        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(Address, serviceUuid, characteristicUuidToUnsubscribe, OnCharacteristicUnsubscribed);
    }
    private void OnCharacteristicUnsubscribed(string characteristicUuid)
    {
        Logger.Log($"Unsubscribed from characteristicUuid {characteristicUuid} for \"{Name}\"");
        SubscribedCharacteristicUuids.Remove(characteristicUuid);
        UnsubscribeToCharacteristics();
    }

    private void OnCharacteristicValue(string characteristicUuid, byte[] data)
    {

        Logger.Log($"Received {data.Length} data from characteristicUuid {characteristicUuid} for \"{Name}\"");
        if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.BatteryLevelCharacteristicUuid)) { OnBatteryLevel(this, data[0]); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.ManufacturerNameStringCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.ManufacturerName, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.ModelNumberStringCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.ModelNumber, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.SerialNumberStringCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.SerialNumber, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.HardwareRevisionStringCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.HardwareRevision, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.FirmwareRevisionCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.FirmwareRevision, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.SoftwareRevisionCharacteristicUuid)) { OnDeviceInformationValue(this, BS_DeviceInformationType.SoftwareRevision, data); }
        else if (BS_BleUtils.AreUuidsEqual(characteristicUuid, BS_BleUtils.RxCharacteristicUuid)) { ParseRxData(data); }
        else { Logger.LogError($"Uncaught characteristicUuid {characteristicUuid}"); }
    }

    public override void Update()
    {
        base.Update();

        if (_timeout <= 0f)
        {
            return;
        }
        _timeout -= Time.deltaTime;
        if (_timeout > 0)
        {
            return;
        }
        _timeout = 0;

        Logger.Log($"Update Stage: {Stage}");

        switch (Stage)
        {
            case BS_BleConnectionStage.Connecting:
                ConnectToPeripheral();
                break;
            case BS_BleConnectionStage.Disconnecting:
                DisconnectPeripheral();
                break;
            case BS_BleConnectionStage.RequestingMtu:
                RequestMtu();
                break;
            case BS_BleConnectionStage.ReadingCharacteristics:
                ReadCharacteristics();
                break;
            case BS_BleConnectionStage.UnsubscribingFromCharacteristics:
                UnsubscribeToCharacteristics();
                break;
            case BS_BleConnectionStage.SubscribingToCharacteristics:
                SubscribeToCharacteristics();
                break;
            case BS_BleConnectionStage.WritingTxCharacteristic:
                WriteTxData();
                break;
            default:
                break;
        }
    }

    private List<byte>? TxData = null;
    public override void SendTxData(List<byte> Data)
    {
        if (!IsConnected)
        {
            Logger.Log($"Not connected");
            return;
        }
        base.SendTxData(Data);
        TxData = Data;
        Stage = BS_BleConnectionStage.WritingTxCharacteristic;
    }

    private void WriteTxData()
    {
        if (TxData == null) { return; }
        var data = TxData.ToArray();
        Logger.Log($"Writing {data.Length} bytes to Tx for \"{Name}\"...");
        var mainCharacteristicUuid = GetFoundServiceUuid(BS_BleUtils.MainServiceUuid);
        var txCharacteristicUuid = GetFoundCharacteristicUuid(BS_BleUtils.TxCharacteristicUuid);
        BluetoothLEHardwareInterface.WriteCharacteristic(Address, mainCharacteristicUuid, txCharacteristicUuid, data, data.Length, true, OnCharacteristicWrite);
        TxData = null;
    }
}
