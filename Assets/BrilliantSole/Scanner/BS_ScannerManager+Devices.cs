using System;
using System.Collections.Generic;
using UnityEngine.Events;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Scanner.DiscoveredDevices;

    [Serializable]
    public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice;
    public DiscoveredDeviceUnityEvent OnExpiredDevice;

    public IReadOnlyDictionary<string, BS_Device> Devices => Scanner.Devices;

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }
}
