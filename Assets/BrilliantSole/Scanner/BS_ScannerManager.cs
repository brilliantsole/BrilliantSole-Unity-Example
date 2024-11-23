using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager", BS_Logger.LogLevel.Log);

    private static readonly BS_BaseScanner Scanner = BS_BleScanner.Instance;

    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Scanner.DiscoveredDevices;

    public void Update() { Scanner.Update(); }

    [System.Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    public BoolUnityEvent OnIsScanning;

    public UnityEvent OnScanStart;
    public UnityEvent OnScanStop;

    [System.Serializable]
    public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice;
    public DiscoveredDeviceUnityEvent OnExpiredDevice;

    private void OnEnable()
    {
        Scanner.OnIsScanning += _OnIsScanning;
        Scanner.OnScanStart += _OnScanStart;
        Scanner.OnScanStop += _OnScanStop;

        Scanner.OnDiscoveredDevice += _OnDiscoveredDevice;
        Scanner.OnExpiredDevice += _OnExpiredDevice;
    }

    private void OnDisable()
    {
        Scanner.OnIsScanning -= _OnIsScanning;
        Scanner.OnScanStart -= _OnScanStart;
        Scanner.OnScanStop -= _OnScanStop;
        if (IsScanning)
        {
            OnScanStop?.Invoke();
            OnIsScanning?.Invoke(false);
            Scanner.StopScan();
        }

        Scanner.OnDiscoveredDevice -= _OnDiscoveredDevice;
        Scanner.OnExpiredDevice -= _OnExpiredDevice;
    }

    private void _OnIsScanning(bool IsScanning) { OnIsScanning?.Invoke(IsScanning); }
    private void _OnScanStart() { OnScanStart?.Invoke(); }
    private void _OnScanStop() { OnScanStop?.Invoke(); }

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }

    private void _OnDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void _OnExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }

    public BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.ConnectToDiscoveredDevice(DiscoveredDevice); }
    public BS_Device DisconnectFromDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.DisconnectFromDiscoveredDevice(DiscoveredDevice); }
}
