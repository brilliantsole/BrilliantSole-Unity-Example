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
            if (device.ConnectionManager is BS_ClientConnectionManager connectionManager)
            {
                device._SetConnectionStatus(BS_ConnectionStatus.Connecting);
                connectionManager.SetIsConnected(true);
            }
            else
            {
                Logger.LogError("failed to cast ConnectionManager to BS_ClientConnectionManager");
            }
        }
    }
}
