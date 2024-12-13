using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class BS_BaseClient
{
    protected readonly Dictionary<string, BS_DiscoveredDevice> _discoveredDevices = new();
    protected readonly Dictionary<string, BS_DiscoveredDevice> _allDiscoveredDevices = new();
    public IReadOnlyDictionary<string, BS_DiscoveredDevice> DiscoveredDevices => _discoveredDevices;

    public event Action<BS_DiscoveredDevice> OnDiscoveredDevice;
    public event Action<BS_DiscoveredDevice> OnExpiredDevice;

    private void AddDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        Logger.Log($"Adding Discovered Device \"{DiscoveredDevice.Id}\"");
        _discoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        _allDiscoveredDevices[DiscoveredDevice.Id] = DiscoveredDevice;
        OnDiscoveredDevice?.Invoke(DiscoveredDevice);
    }
    private void RemoveDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        Logger.Log($"Removing Discovered Device \"{DiscoveredDevice.Id}\"");

        if (_discoveredDevices.ContainsKey(DiscoveredDevice.Id))
        {
            Logger.Log($"removing expired discovered device \"{DiscoveredDevice.Id}\"");
            _discoveredDevices.Remove(DiscoveredDevice.Id);
            OnExpiredDevice?.Invoke(DiscoveredDevice);
        }
    }

    private void ParseDiscoveredDevice(in byte[] data)
    {
        Logger.Log($"parsing DiscoveredDevice ({data.Length} bytes)");

        var nullableDiscoveredDeviceJson = BS_DiscoveredDeviceJson.Parse(data);
        if (nullableDiscoveredDeviceJson == null)
        {
            Logger.LogError("failed to parse discoveredDevice");
            return;
        }
        var discoveredDeviceJson = (BS_DiscoveredDeviceJson)nullableDiscoveredDeviceJson;

        if (_allDiscoveredDevices.TryGetValue(discoveredDeviceJson.bluetoothId, out BS_DiscoveredDevice discoveredDevice))
        {
            discoveredDevice.Update(discoveredDeviceJson);
            AddDiscoveredDevice(discoveredDevice);
        }
        else
        {
            AddDiscoveredDevice(new BS_DiscoveredDevice(discoveredDeviceJson));
        }
    }

    private void ParseExpiredDiscoveredDevice(in byte[] data)
    {
        Logger.Log($"parsing ExpiredDiscoveredDevice ({data.Length} bytes)");

        var bluetoothId = Encoding.UTF8.GetString(data);
        Logger.Log($"expired bluetoothId {bluetoothId}");

        if (_discoveredDevices.TryGetValue(bluetoothId, out BS_DiscoveredDevice discoveredDevice))
        {
            RemoveDiscoveredDevice(discoveredDevice);
        }
        else
        {
            Logger.Log($"couldn't find discovered device with bluetoothId {bluetoothId}");
        }
    }
}
