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

    public BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.ConnectToDiscoveredDevice(DiscoveredDevice); }
    public BS_Device DisconnectFromDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.DisconnectFromDiscoveredDevice(DiscoveredDevice); }
    public BS_Device ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { return Scanner.ToggleConnectionToDiscoveredDevice(DiscoveredDevice); }

    public IReadOnlyDictionary<string, BS_Device> Devices => Scanner.Devices;

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }
}
