using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class ScannerManagerUnityEvent : UnityEvent<IBS_ScannerManager> { }
[Serializable]
public class ScannerManagerBoolUnityEvent : UnityEvent<IBS_ScannerManager, bool> { }
[Serializable]
public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }

public interface IBS_ScannerManager
{
    IBS_Scanner Scanner { get; }

    bool IsScanning { get; }
    bool IsScanningAvailable { get; }

    ScannerManagerBoolUnityEvent OnIsScanning { get; }
    ScannerManagerUnityEvent OnScanStart { get; }
    ScannerManagerUnityEvent OnScanStop { get; }

    void StartScan();
    void StopScan();
    void ToggleScan();

    ScannerManagerBoolUnityEvent OnIsScanningAvailable { get; }
    ScannerManagerUnityEvent OnScanningIsAvailable { get; }
    ScannerManagerUnityEvent OnScanningIsUnavailable { get; }

    IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices { get; }

    DiscoveredDeviceUnityEvent OnDiscoveredDevice { get; }
    DiscoveredDeviceUnityEvent OnExpiredDevice { get; }

    IReadOnlyDictionary<string, BS_Device> Devices { get; }
}
