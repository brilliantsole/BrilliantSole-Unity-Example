using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class BS_BaseScanner
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseScanner", BS_Logger.LogLevel.Log);

    protected virtual void Initialize() { }
    protected virtual void DeInitialize() { }

    public virtual bool IsAvailable
    {
        get
        {
            return false;
        }
    }

    public event Action<bool> OnIsScanning;
    public event Action OnScanStart;
    public event Action OnScanStop;

    [SerializeField]
    private bool _isScanning;
    public bool IsScanning
    {
        get => _isScanning;
        protected set
        {
            if (_isScanning != value)
            {
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

    public event Action<BS_DiscoveredDevice> OnDiscoveredDevice;
    public event Action<BS_DiscoveredDevice> OnExpiredDevice;

    protected void AddDiscoveredDevice(in BS_DiscoveredDevice DiscoveredDevice)
    {
        Logger.Log($"Adding Discovered Device \"{DiscoveredDevice.Id}\"");
        _discoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        _allDiscoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        OnDiscoveredDevice?.Invoke(DiscoveredDevice);
    }
    private void RemoveDiscoveredDevice(in BS_DiscoveredDevice DiscoveredDevice)
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

    public virtual BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        if (!_discoveredDevices.ContainsKey(DiscoveredDevice.Id))
        {
            throw new ArgumentException($"Invalid DiscoveredDevice \"{DiscoveredDevice.Name}\"");
        }
        // FILL - find existing device with that id from DeviceManager
        return new();
    }
}