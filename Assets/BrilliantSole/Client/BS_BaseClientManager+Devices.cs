using System.Collections.Generic;
using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Client.DiscoveredDevices;

    [field: SerializeField]
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice { get; private set; } = new();

    [field: SerializeField]
    public DiscoveredDeviceUnityEvent OnExpiredDevice { get; private set; } = new();

    public IReadOnlyDictionary<string, BS_Device> Devices => Client.Devices;

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }
}
