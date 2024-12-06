using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public abstract class BS_BaseScanner
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseScanner");

    protected virtual void Initialize()
    {
        Logger.Log("Initializing...");
    }
    protected virtual void DeInitialize()
    {
        Logger.Log("DeInitializing...");
    }

    public virtual bool IsAvailable
    {
        get
        {
            return false;
        }
    }

    public event Action<bool>? OnIsScanning;
    public event Action? OnScanStart;
    public event Action? OnScanStop;

    [SerializeField]
    private bool _isScanning;
    public bool IsScanning
    {
        get => _isScanning;
        protected set
        {
            if (_isScanning == value) { return; }
            Logger.Log($"Updating IsScanning to {value}");
            _isScanning = value;
            OnIsScanning?.Invoke(IsScanning);
            if (IsScanning)
            {
                OnScanStart?.Invoke();
            }
            else
            {
                OnScanStop?.Invoke();
            }
        }
    }

    public virtual bool StartScan()
    {
        if (IsScanning)
        {
            Logger.Log("Already scanning");
            return false;
        }
        if (!IsAvailable)
        {
            Logger.LogError("Scanning is not available");
            return false;
        }
        _discoveredDevices.Clear();
        _devices.Clear();
        Logger.Log("Starting scan.");
        return true;
    }

    public virtual bool StopScan()
    {
        if (!IsScanning)
        {
            Logger.Log("Already not scanning");
            return false;
        }
        Logger.Log("Stopping scan");
        return true;
    }

    public void ToggleScan()
    {
        Logger.Log("Toggling scan");

        if (IsScanning)
        {
            StopScan();
        }
        else
        {
            StartScan();
        }
    }

    protected readonly Dictionary<string, BS_DiscoveredDevice> _discoveredDevices = new();
    protected readonly Dictionary<string, BS_DiscoveredDevice> _allDiscoveredDevices = new();
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => _discoveredDevices;

    protected readonly Dictionary<string, BS_Device> _devices = new();
    protected readonly Dictionary<string, BS_Device> _allDevices = new();
    public IReadOnlyDictionary<string, BS_Device> Devices => _devices;

    public event Action<BS_DiscoveredDevice>? OnDiscoveredDevice;
    public event Action<BS_DiscoveredDevice>? OnExpiredDevice;

    protected void AddDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        Logger.Log($"Adding Discovered Device \"{DiscoveredDevice.Id}\"");
        _discoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        _allDiscoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        OnDiscoveredDevice?.Invoke(DiscoveredDevice);
    }
    private void RemoveDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        Logger.Log($"Removing Discovered Device \"{DiscoveredDevice.Id}\"");

        if (_discoveredDevices.ContainsKey(DiscoveredDevice.Id))
        {
            Logger.Log($"removing expired discovered device \"{DiscoveredDevice.Name}\"");
            _discoveredDevices.Remove(DiscoveredDevice.Id);
            OnExpiredDevice?.Invoke(DiscoveredDevice);
        }
    }

    public virtual void Update()
    {
        if (IsScanning)
        {
            RemoveExpiredDiscoveredDevices();
        }
    }

    readonly private int DiscoveredDeviceExpirationTime = 5;
    private void RemoveExpiredDiscoveredDevices()
    {
        List<string> deviceIdsToRemove = new();

        foreach (var discoveredDevicePair in _discoveredDevices)
        {
            if (discoveredDevicePair.Value.TimeSinceLastUpdate.TotalSeconds > DiscoveredDeviceExpirationTime)
            {
                Logger.Log($"\"{discoveredDevicePair.Value.Name}\" device has expired");
                deviceIdsToRemove.Add(discoveredDevicePair.Key);
            }
        }

        foreach (var discoveredDeviceId in deviceIdsToRemove)
        {
            RemoveDiscoveredDevice(_discoveredDevices[discoveredDeviceId]);
        }
    }

    protected virtual BS_Device CreateDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device Device = new();
        Logger.Log($"creating device for {discoveredDevice.Name}...");
        _allDevices[discoveredDevice.Id] = Device;
        Device.OnIsConnected += (BS_Device device, bool isConnected) =>
        {
            if (isConnected) { _devices[discoveredDevice.Id] = device; }
            else { _devices.Remove(discoveredDevice.Id); }
        };
        return Device;
    }

    private BS_Device? GetDeviceByDiscoveredDevice(BS_DiscoveredDevice discoveredDevice, bool CreateIfNotFound = false)
    {
        if (!_discoveredDevices.ContainsKey(discoveredDevice.Id))
        {
            throw new ArgumentException($"Invalid discoveredDevice \"{discoveredDevice.Name}\"");
        }
        if (!_allDevices.ContainsKey(discoveredDevice.Id))
        {
            Logger.Log($"no device found for {discoveredDevice.Name}");
            if (CreateIfNotFound)
            {
                CreateDevice(discoveredDevice);
            }
            else
            {
                return null;
            }
        }
        return _allDevices[discoveredDevice.Id];
    }

    public virtual BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        return GetDeviceByDiscoveredDevice(discoveredDevice, true)!;
    }

    public BS_Device? DisconnectFromDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device? device = GetDeviceByDiscoveredDevice(discoveredDevice);
        device?.Disconnect();
        return device;
    }

    public BS_Device? ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device device = GetDeviceByDiscoveredDevice(discoveredDevice, true)!;
        device.ToggleConnection();
        return device;
    }
}