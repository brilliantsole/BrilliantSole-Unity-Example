using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class ScannerUnityEvent : UnityEvent<IBS_Scanner> { }
[Serializable]
public class ScannerBoolUnityEvent : UnityEvent<IBS_Scanner, bool> { }
[Serializable]
public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }

public interface IBS_ScannerManager
{
    IBS_Scanner Scanner { get; }

    bool IsScanning { get; }
    bool IsScanningAvailable { get; }

    ScannerBoolUnityEvent OnIsScanning { get; }
    ScannerUnityEvent OnScanStart { get; }
    ScannerUnityEvent OnScanStop { get; }

    void StartScan();
    void StopScan();
    void ToggleScan();

    ScannerBoolUnityEvent OnIsScanningAvailable { get; }
    ScannerUnityEvent OnScanningIsAvailable { get; }
    ScannerUnityEvent OnScanningIsUnavailable { get; }

    IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices { get; }

    DiscoveredDeviceUnityEvent OnDiscoveredDevice { get; }
    DiscoveredDeviceUnityEvent OnExpiredDevice { get; }

    IReadOnlyDictionary<string, BS_Device> Devices { get; }
}
