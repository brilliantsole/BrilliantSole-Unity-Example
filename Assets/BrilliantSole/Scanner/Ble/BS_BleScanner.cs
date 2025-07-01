using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

#nullable enable

public class BS_BleScanner : BS_BaseScanner<BS_BleScanner>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleScanner", BS_Logger.LogLevel.Log);

    public override bool IsScanningAvailable
    {
        get
        {
            return true;
        }
    }

    private bool _isInitialized = false;
    private void InitializeBle()
    {
        if (_isInitialized)
        {
            Logger.LogWarning("Ble already initialized");
            return;
        }
        Logger.Log("initializing Ble");
        BluetoothLEHardwareInterface.Initialize(true, false, OnBleInitializationSuccess, OnBleInitializationError);
    }

    private void OnBleInitializationSuccess()
    {
        Logger.Log("Ble initialized");
        _isInitialized = true;
        _scanTimeout = _scanTimeoutDuration;
        StartScan();
    }
    private void OnBleInitializationError(string error)
    {
        Logger.LogError($"Initialization error: {error}");
        _isInitialized = false;
        StopScan();

        if (error.Contains("Bluetooth LE Not Enabled"))
        {
            BluetoothLEHardwareInterface.BluetoothEnable(true);
        }
    }

    private void DeInitializeBle()
    {
        if (!_isInitialized)
        {
            Logger.Log("Ble already not initialized");
            return;
        }
        foreach (var connectionManager in _connectionManagers.Values)
        {
            connectionManager.Disconnect();
        }
        Logger.Log("deinitializing Ble");
        BluetoothLEHardwareInterface.DeInitialize(OnBleDeInitialization);
    }
    private void OnBleDeInitialization()
    {
        Logger.Log("Ble deInitialized");
        _isInitialized = false;
        StopScan();
    }

    public override bool StartScan()
    {
        if (!base.StartScan())
        {
            return false;
        }
        if (!_isInitialized)
        {
            InitializeBle();
            return false;
        }
        else
        {
            _didScan = false;
            IsScanning = true;
            return true;
        }
    }

    public override bool StopScan()
    {
        if (!base.StopScan())
        {
            return false;
        }
        Logger.Log("Stopping Ble Scan...");
        BluetoothLEHardwareInterface.StopScan();
        IsScanning = false;
        return true;
    }

    private readonly float _scanTimeoutDuration = 0.2f;
    private float _scanTimeout = 0;
    private bool _didScan = false;
    private void Scan()
    {
        if (_didScan) { return; }
        _scanTimeout -= Time.deltaTime;
        if (_scanTimeout >= 0) { return; }
        _didScan = true;
        Logger.Log("Scanning for devices...");
        BluetoothLEHardwareInterface.RetrieveListOfPeripheralsWithServices(BS_BleUtils.ScanServiceUuids, OnDiscoveredBleDevice);
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(BS_BleUtils.ScanServiceUuids, OnDiscoveredBleDevice, OnDiscoveredBleDeviceData, true);
    }

    private void OnDiscoveredBleDevice(string address, string name)
    {
        Logger.Log($"Discovered device \"{name}\" with address {address}");
        if (_allDiscoveredDevices.TryGetValue(address, out var discoveredDevice))
        {
            discoveredDevice.Update(name, null, null);
            AddDiscoveredDevice(discoveredDevice);
        }
        else
        {
            AddDiscoveredDevice(new BS_DiscoveredDevice(this, address, name, null, null));
        }
    }
    private void OnDiscoveredBleDeviceData(string address, string name, int rssi, byte[] data)
    {
        Logger.Log($"Discovered \"{name}\" with address {address}, RSSI {rssi}, and {data.Length} data");

        BS_DeviceType? deviceType = null;
        if (data.Length >= 3)
        {
            deviceType = (BS_DeviceType)data[2];
            Logger.Log($"Device \"{name}\" is type \"{deviceType}\"");
        }
        IPAddress? ipAddress = null;
        bool? isWifiSecure = null;
        if (data.Length >= 3 + 4)
        {
            byte[] ipBytes = new byte[4];
            Array.Copy(data, 3, ipBytes, 0, 4);
            ipAddress = new IPAddress(ipBytes);
            Logger.Log($"Device \"{name}\" ipAddress: \"{ipAddress}\"");
        }
        if (data.Length >= 3 + 4 + 1)
        {
            isWifiSecure = data[3 + 4] != 0;
            Logger.Log($"Device \"{name}\" isWifiSecure? {isWifiSecure}");
        }

        // TODO: - integrate ipAddress and isWifiSecure
        if (_allDiscoveredDevices.TryGetValue(address, out var discoveredDevice))
        {
            discoveredDevice.Update(name, deviceType, rssi);
            AddDiscoveredDevice(discoveredDevice);
        }
        else
        {
            AddDiscoveredDevice(new BS_DiscoveredDevice(this, address, name, deviceType, rssi));
        }
    }

    public override void Update()
    {
        base.Update();

        if (IsScanning)
        {
            Scan();
        }

        foreach (var connectionManager in _connectionManagers.Values)
        {
            if (connectionManager.Stage != BS_BleConnectionStage.None)
            {
                connectionManager.Update();
                break;
            }
        }
    }

    protected override void DeInitialize()
    {
        base.DeInitialize();
        DeInitializeBle();
    }

    private readonly Dictionary<string, BS_BleConnectionManager> _connectionManagers = new();
    protected override BS_Device CreateDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device Device = base.CreateDevice(discoveredDevice);
        BS_BleConnectionManager ConnectionManager = new()
        {
            DiscoveredDevice = discoveredDevice
        };
        _connectionManagers[discoveredDevice.Id] = ConnectionManager;
        Device.ConnectionManager = ConnectionManager;
        return Device;
    }
}
