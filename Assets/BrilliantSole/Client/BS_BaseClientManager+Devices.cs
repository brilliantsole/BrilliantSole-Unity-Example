using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => Client.DiscoveredDevices;

    [Serializable]
    public class DiscoveredDeviceUnityEvent : UnityEvent<BS_DiscoveredDevice> { }
    public DiscoveredDeviceUnityEvent OnDiscoveredDevice;
    public DiscoveredDeviceUnityEvent OnExpiredDevice;

    public IReadOnlyDictionary<string, BS_Device> Devices => Client.Devices;

    private void onDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnDiscoveredDevice?.Invoke(DiscoveredDevice); }
    private void onExpiredDevice(BS_DiscoveredDevice DiscoveredDevice) { OnExpiredDevice?.Invoke(DiscoveredDevice); }
}
