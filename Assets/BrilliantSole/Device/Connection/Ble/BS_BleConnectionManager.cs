using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public class BS_BleConnectionManager : BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleConnectionManager", BS_Logger.LogLevel.Log);

    public override BS_ConnectionType Type => BS_ConnectionType.Ble;

    public BS_DiscoveredDevice? DiscoveredDevice;
    public string Address => DiscoveredDevice?.Id ?? "";
    public string Name => DiscoveredDevice?.Name ?? "";

    private void Reset()
    {
        ResetFoundUuids();
        Stage = BleConnectionStage.None;
    }

    private readonly HashSet<string> FoundServiceUuids = new();
    private readonly HashSet<string> FoundCharacteristicUuids = new();
    private void ResetFoundUuids()
    {
        FoundServiceUuids.Clear();
        FoundCharacteristicUuids.Clear();
        ReadCharacteristicUuids.Clear();
        SubscribedCharacteristicUuids.Clear();
    }

    private bool CheckFoundAllServiceUuids()
    {
        bool FoundAllServiceUuids = true;
        foreach (string serviceUuid in BS_BleUtils.AllServiceUuids)
        {
            if (!FoundServiceUuids.Contains(serviceUuid))
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
        foreach (string characteristicUuid in BS_BleUtils.AllCharacteristicUuids)
        {
            if (!FoundCharacteristicUuids.Contains(characteristicUuid))
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
        if (Stage == BleConnectionStage.WaitForUuids && DidFindAllUuids)
        {
            Logger.Log("Got all Uuids");
            Stage = BleConnectionStage.RequestingMtu;
        }
    }

    enum BleConnectionStage
    {
        None,
        Connecting,
        WaitForUuids,
        RequestingMtu,
        ReadingCharacteristics,
        SubscribingToCharacteristics,
        Disconnecting,
    }

    [SerializeField]
    private float _timeout = 0f;
    private void UpdateTimeout()
    {
        _timeout = _Stage switch
        {
            BleConnectionStage.Connecting => 0.5f,
            BleConnectionStage.RequestingMtu => 0.5f,
            BleConnectionStage.SubscribingToCharacteristics => 0.1f,
            BleConnectionStage.ReadingCharacteristics => 0.1f,
            _ => 0f,
        };
        Logger.Log($"timeout: {_timeout}");
    }

    [SerializeField]
    private BleConnectionStage _Stage = BleConnectionStage.None;
    private BleConnectionStage Stage
    {
        get => _Stage;
        set
        {
            if (_Stage == value) { return; }
            Logger.Log($"Updating BleConnectionStage to {value}");
            _Stage = value;
            UpdateTimeout();
        }
    }

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue)
        {
            return;
        }
        Stage = BleConnectionStage.Connecting;
    }
    private void ConnectToPeripheral()
    {
        Logger.Log($"Connecting to peripheral \"{Name}\"");
        BluetoothLEHardwareInterface.ConnectToPeripheral(Address, OnConnectToPeripheral, OnPeripheralService, OnPeripheralCharacteristic, OnPeripheralDisconnect);
    }

    private void OnConnectToPeripheral(string address)
    {
        Logger.Log($"Connected to peripheral {Address}");
        Stage = BleConnectionStage.WaitForUuids;
    }
    private void OnPeripheralService(string address, string serviceUuid)
    {
        Logger.Log($"Got Service {serviceUuid}");
        FoundServiceUuids.Add(serviceUuid);
        CheckDidFindAllUuids();
    }
    private void OnPeripheralCharacteristic(string address, string serviceUuid, string characteristicUuid)
    {
        Logger.Log($"Got Characteristic {characteristicUuid} for service {serviceUuid}");
        FoundCharacteristicUuids.Add(characteristicUuid);
        FoundServiceUuids.Add(serviceUuid);
        CheckDidFindAllUuids();
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
        Stage = BleConnectionStage.Disconnecting;
        DisconnectPeripheral();
    }
    private void DisconnectPeripheral()
    {
        Logger.Log($"Disconnecting from peripheral \"{Name}\"");
        BluetoothLEHardwareInterface.DisconnectPeripheral(Address, OnDisconnectPeripheral);
    }

    private void OnDisconnectPeripheral(string address)
    {
        Logger.Log("Disconnected from \"{Name}\"");
        Status = BS_ConnectionStatus.NotConnected;
        Stage = BleConnectionStage.None;
    }

    private void RequestMtu()
    {
        Logger.Log($"Requesting Mtu from \"{Name}\"");
        BluetoothLEHardwareInterface.RequestMtu(Address, 498, OnPeripheralMtu);
    }

    private void OnPeripheralMtu(string address, int mtu)
    {
        Logger.Log($"Updated Mtu of \"{Name}\" to {mtu}");
        Stage = BleConnectionStage.ReadingCharacteristics;
    }


    private readonly HashSet<string> ReadCharacteristicUuids = new();
    private string? GetNextCharacteristicUuidToRead()
    {
        string? nextCharacteristicUuidToRead = null;
        foreach (string characteristicUuid in BS_BleUtils.ReadableCharacteristicUuids)
        {
            if (!ReadCharacteristicUuids.Contains(characteristicUuid))
            {
                nextCharacteristicUuidToRead = characteristicUuid;
                break;
            }
        }
        return nextCharacteristicUuidToRead;
    }

    private void ReadCharacteristics()
    {
        string? characteristicUuidToRead = GetNextCharacteristicUuidToRead();
        if (characteristicUuidToRead == null)
        {
            Logger.Log("Read all Characteristics");
            Stage = BleConnectionStage.SubscribingToCharacteristics;
            return;
        }
        Logger.Log($"nextCharacteristicUuidToRead {characteristicUuidToRead}");

        string? serviceUuid = BS_BleUtils.GetServiceUuid(characteristicUuidToRead);
        if (serviceUuid == null)
        {
            Logger.LogError($"Unable to find serviceUuid for characteristicUuidToRead {characteristicUuidToRead}");
            return;
        }
        Logger.Log($"reading characteristicUuidToRead {characteristicUuidToRead} of serviceUuid {serviceUuid}...");
        BluetoothLEHardwareInterface.ReadCharacteristic(Address, serviceUuid, characteristicUuidToRead, OnCharacteristicRead);
    }
    private void OnCharacteristicRead(string characteristicUuid, byte[] bytes)
    {
        Logger.Log($"Read {bytes.Length} bytes from characteristicUuid {characteristicUuid} for \"{Name}\"");
        // FILL - dispatch to main device
        ReadCharacteristicUuids.Add(characteristicUuid);
        ReadCharacteristics();
    }


    private readonly HashSet<string> SubscribedCharacteristicUuids = new();
    private string? GetNextCharacteristicUuidToSubscribe()
    {
        string? nextCharacteristicUuidToSubscribe = null;
        foreach (string characteristicUuid in BS_BleUtils.NotifiableCharacteristicUuids)
        {
            if (!SubscribedCharacteristicUuids.Contains(characteristicUuid))
            {
                nextCharacteristicUuidToSubscribe = characteristicUuid;
                break;
            }
        }
        return nextCharacteristicUuidToSubscribe;
    }
    private void SubscribeToCharacteristics()
    {
        string? characteristicUuidToSubscribe = GetNextCharacteristicUuidToSubscribe();
        if (characteristicUuidToSubscribe == null)
        {
            Logger.Log("Subscribed to all Characteristics");
            Stage = BleConnectionStage.None;
            Status = BS_ConnectionStatus.Connected;
            return;
        }
        Logger.Log($"nextCharacteristicUuidToSubscribe {characteristicUuidToSubscribe}");

        string? serviceUuid = BS_BleUtils.GetServiceUuid(characteristicUuidToSubscribe);
        if (serviceUuid == null)
        {
            Logger.LogError($"Unable to find serviceUuid for characteristicUuidToSubscribe {characteristicUuidToSubscribe}");
            return;
        }
        Logger.Log($"subscribing to characteristicUuidToSubscribe {characteristicUuidToSubscribe} of serviceUuid {serviceUuid}...");
        BluetoothLEHardwareInterface.SubscribeCharacteristic(Address, serviceUuid, characteristicUuidToSubscribe, OnCharacteristicSubscribed, OnCharacteristicNotify);
    }
    private void UnsubscribeToCharacteristics()
    {
        // FILL
    }
    private void OnCharacteristicNotify(string characteristicUuid, byte[] bytes)
    {
        Logger.Log($"Was notified {bytes.Length} bytes from characteristicUuid {characteristicUuid} for \"{Name}\"");
        // FILL - dispatch to main device
    }
    private void OnCharacteristicSubscribed(string characteristicUuid)
    {
        Logger.Log($"Subscribed to characteristicUuid {characteristicUuid} for \"{Name}\"");
        SubscribedCharacteristicUuids.Add(characteristicUuid);
        SubscribeToCharacteristics();
    }
    private void OnCharacteristicUnsubscribed(string characteristicUuid)
    {
        Logger.Log($"Unsubscribed from characteristicUuid {characteristicUuid} for \"{Name}\"");
        SubscribedCharacteristicUuids.Remove(characteristicUuid);
        UnsubscribeToCharacteristics();
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
            case BleConnectionStage.Connecting:
                ConnectToPeripheral();
                //UpdateTimeout();
                break;
            case BleConnectionStage.Disconnecting:
                DisconnectPeripheral();
                break;
            case BleConnectionStage.RequestingMtu:
                RequestMtu();
                break;
            case BleConnectionStage.ReadingCharacteristics:
                ReadCharacteristics();
                break;
            case BleConnectionStage.SubscribingToCharacteristics:
                SubscribeToCharacteristics();
                break;
            default:
                break;
        }
    }
}
