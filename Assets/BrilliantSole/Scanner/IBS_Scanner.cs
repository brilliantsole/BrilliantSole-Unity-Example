using System;
using System.Collections.Generic;

#nullable enable

public interface IBS_Scanner
{
    public bool IsScanningAvailable { get; }
    bool IsScanning { get; }

    event Action<IBS_Scanner, bool>? OnIsScanning;
    event Action<IBS_Scanner, bool>? OnIsScanningAvailable;
    event Action<IBS_Scanner>? OnScanningIsAvailable;
    event Action<IBS_Scanner>? OnScanningIsUnavailable;
    event Action<IBS_Scanner>? OnScanStart;
    event Action<IBS_Scanner>? OnScanStop;

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

    event Action<BS_DiscoveredDevice>? OnDiscoveredDevice;
    event Action<BS_DiscoveredDevice>? OnExpiredDevice;

    BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
    BS_Device? DisconnectFromDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
    BS_Device? ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice);
}