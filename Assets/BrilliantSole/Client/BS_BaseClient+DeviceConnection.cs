using System;
using UnityEngine;

#nullable enable

public partial class BS_BaseClient
{
    private BS_Device CreateDevice(string bluetoothId)
    {
        BS_Device device = new();
        Logger.Log($"creating device for bluetoothId {bluetoothId}...");
        SetupDevice(device, bluetoothId);
        return device;
    }
    private BS_Device CreateDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device device = new(discoveredDevice);
        Logger.Log($"creating device for {discoveredDevice.Name}...");
        SetupDevice(device, discoveredDevice.Id);
        return device;
    }

    private void SetupDevice(BS_Device device, string bluetoothId)
    {
        _allDevices[bluetoothId] = device;
        device.OnIsConnected += (device, isConnected) =>
        {
            if (isConnected) { _devices[bluetoothId] = device; }
            else { _devices.Remove(bluetoothId); }
        };
        // FILL - add connectionManager
        // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/c273625334a365a519b771b8fd2ea4b563514713/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_BaseClient.cpp#L352
    }

    private BS_Device? GetDeviceByDiscoveredDevice(BS_DiscoveredDevice discoveredDevice, bool CreateIfNotFound = false)
    {
        if (!_discoveredDevices.ContainsKey(discoveredDevice.Id))
        {
            throw new ArgumentException($"Invalid discoveredDevice \"{discoveredDevice.Name}\"");
        }
        if (!_allDevices.ContainsKey(discoveredDevice.Id))
        {
            Logger.Log($"no device found for {discoveredDevice.Name}");
            if (CreateIfNotFound)
            {
                CreateDevice(discoveredDevice);
            }
            else
            {
                return null;
            }
        }
        return _allDevices[discoveredDevice.Id];
    }

    public BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        return GetDeviceByDiscoveredDevice(discoveredDevice, true)!;
    }

    public BS_Device? DisconnectFromDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device? device = GetDeviceByDiscoveredDevice(discoveredDevice);
        device?.Disconnect();
        return device;
    }

    public BS_Device? ToggleConnectionToDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        BS_Device device = GetDeviceByDiscoveredDevice(discoveredDevice, true)!;
        device.ToggleConnection();
        return device;
    }
}