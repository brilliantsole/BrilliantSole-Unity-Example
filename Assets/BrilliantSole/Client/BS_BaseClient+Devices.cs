using System.Collections.Generic;
using UnityEngine;

public partial class BS_BaseClient
{
    private readonly Dictionary<string, BS_Device> _devices = new();
    private readonly Dictionary<string, BS_Device> _allDevices = new();
    public IReadOnlyDictionary<string, BS_Device> Devices => _devices;

    private void ParseConnectedDevices(in byte[] data)
    {
        Logger.Log($"parsing connected devices ({data.Length} bytes...)");

        var connectedDeviceBluetoothIdsString = BS_StringUtils.GetString(data, true);
        Logger.Log($"connectedDeviceBluetoothIdsString: {connectedDeviceBluetoothIdsString}");

        var connectedDeviceBluetoothIds = JsonUtility.FromJson<BS_ConnectedDevicesJson>(connectedDeviceBluetoothIdsString).connectedDevices;
        foreach (var connectedDeviceBluetoothId in connectedDeviceBluetoothIds)
        {
            Logger.Log($"connectedDeviceBluetoothId: {connectedDeviceBluetoothId}");
            var device = CreateDevice(connectedDeviceBluetoothId);
            // FILL - assign connectionManager to connected
            // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/c273625334a365a519b771b8fd2ea4b563514713/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_BaseClient.cpp#L418
        }
    }
}
