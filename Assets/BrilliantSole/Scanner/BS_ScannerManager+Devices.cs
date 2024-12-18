using System.Collections.Generic;
using UnityEngine;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Scanner.DiscoveredDevices;

    [field: SerializeField]
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice { get; private set; } = new();

    [field: SerializeField]
    public DiscoveredDeviceUnityEvent OnExpiredDevice { get; private set; } = new();

    public IReadOnlyDictionary<string, BS_Device> Devices => Scanner.Devices;

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }
}
