using System;
using UnityEngine;

#nullable enable

public partial class BS_BaseClient
{
    private BS_Device CreateDevice(string bluetoothId)
    {
        var device = GetDeviceByBluetoothId(bluetoothId);
        if (device != null)
        {
            Logger.Log($"already created device for {bluetoothId}");
            return device;
        }
        device = new();
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
        var connectionManager = new BS_ClientConnectionManager
        {
            Client = this,
            bluetoothId = bluetoothId
        };
        device.ConnectionManager = connectionManager;
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
    private BS_Device? GetDeviceByBluetoothId(string bluetoothId)
    {
        BS_Device? foundDevice = null;
        foreach (var pair in _allDevices)
        {
            if (pair.Key == bluetoothId)
            {
                foundDevice = pair.Value;
                break;
            }
        }
        return foundDevice;
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
