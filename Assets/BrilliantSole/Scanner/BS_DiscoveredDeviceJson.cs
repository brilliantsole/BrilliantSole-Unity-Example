using System;
using System.Text;
using UnityEngine;
using static BS_DeviceType;

#nullable enable

[Serializable]
public readonly struct BS_DiscoveredDeviceJson
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDeviceJson");

    public readonly string bluetoothId;
    public readonly string name;
    public readonly int rssi;
    public readonly string? deviceType;
    public BS_DeviceType? DeviceType => deviceType switch
    {
        "leftInsole" => LeftInsole,
        "rightInsole" => RightInsole,
        _ => null
    };

    public static BS_DiscoveredDeviceJson? Parse(in byte[] data)
    {
        Logger.Log($"parsing json ({data.Length} bytes)...");
        var jsonString = Encoding.UTF8.GetString(data);

        if (jsonString == null)
        {
            Logger.LogError($"failed to parse json string");
            return null;
        }
        Logger.Log($"parsing json string {jsonString}...");
        var discoveredDeviceJson = JsonUtility.FromJson<BS_DiscoveredDeviceJson>(jsonString);
        return discoveredDeviceJson;
    }
}

