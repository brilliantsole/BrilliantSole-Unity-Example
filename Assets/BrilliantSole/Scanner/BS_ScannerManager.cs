using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager", BS_Logger.LogLevel.Log);

    private static BS_BaseScanner Scanner => BS_BleScanner.Instance;

    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Scanner.DiscoveredDevices;

    public void Update() { Scanner.Update(); }

    [Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    public BoolUnityEvent OnIsScanning;

    public UnityEvent OnScanStart;
    public UnityEvent OnScanStop;

    [Serializable]
    public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice;
    public DiscoveredDeviceUnityEvent OnExpiredDevice;

    private void OnEnable()
    {
        Scanner.OnIsScanning += onIsScanning;
        Scanner.OnScanStart += onScanStart;
        Scanner.OnScanStop += onScanStop;

        Scanner.OnDiscoveredDevice += onDiscoveredDevice;
        Scanner.OnExpiredDevice += onExpiredDevice;
    }

    private void OnDisable()
    {
        Scanner.OnIsScanning -= onIsScanning;
        Scanner.OnScanStart -= onScanStart;
        Scanner.OnScanStop -= onScanStop;
        if (IsScanning)
        {
            OnScanStop?.Invoke();
            OnIsScanning?.Invoke(false);
            Scanner.StopScan();
        }

        Scanner.OnDiscoveredDevice -= onDiscoveredDevice;
        Scanner.OnExpiredDevice -= onExpiredDevice;
    }

    private void onIsScanning(bool IsScanning) { OnIsScanning?.Invoke(IsScanning); }
    private void onScanStart() { OnScanStart?.Invoke(); }
    private void onScanStop() { OnScanStop?.Invoke(); }

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }

    public BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.ConnectToDiscoveredDevice(DiscoveredDevice); }
    public BS_Device DisconnectFromDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.DisconnectFromDiscoveredDevice(DiscoveredDevice); }
    public BS_Device ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.ToggleConnectionToDiscoveredDevice(DiscoveredDevice); }

    public IReadOnlyDictionary<string, BS_Device> Devices => Scanner.Devices;


#if UNITY_EDITOR
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        BS_BleScanner.DestroyInstance();
    }
#endif
}
