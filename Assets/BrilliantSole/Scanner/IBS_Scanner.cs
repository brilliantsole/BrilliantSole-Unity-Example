using System;
using System.Collections.Generic;

#nullable enable

public interface IBS_Scanner
{
    public bool IsScanningAvailable { get; }
    bool IsScanning { get; }

    public delegate void ScannerDelegate(IBS_Scanner scanner);
    public delegate void IsScanningDelegate(IBS_Scanner scanner, bool isScanning);
    public delegate void IsScanningAvailableDelegate(IBS_Scanner scanner, bool isScanningAvailable);

    public event IsScanningDelegate? OnIsScanning;
    public event IsScanningAvailableDelegate? OnIsScanningAvailable;
    public event ScannerDelegate? OnScanningIsAvailable;
    public event ScannerDelegate? OnScanningIsUnavailable;
    public event ScannerDelegate? OnScanStart;
    public event ScannerDelegate? OnScanStop;

    bool StartScan();
    bool StopScan();
    void ToggleScan()
    {
        if (IsScanning)
        {
            StopScan();
        }
        else
        {
            StartScan();
        }
    }

    void Update();

    IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices { get; }
    IReadOnlyDictionary<string, BS_Device> Devices { get; }

    public delegate void DiscoveredDeviceDelegate(BS_DiscoveredDevice discoveredDevice);
    public event DiscoveredDeviceDelegate? OnDiscoveredDevice;
    public delegate void ExpiredDiscoveredDeviceDelegate(BS_DiscoveredDevice expiredDiscoveredDevice);
    public event ExpiredDiscoveredDeviceDelegate? OnExpiredDevice;

    BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
    BS_Device? DisconnectFromDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
    BS_Device? ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
}