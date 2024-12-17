using System;
using UnityEngine;
using static BS_DeviceType;

[Serializable]
public struct BS_DiscoveredDeviceJson
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDeviceJson");

    public string bluetoothId;
    public string name;
    public int rssi;
    public string deviceType;
    public readonly BS_DeviceType? DeviceType => deviceType switch
    {
        "leftInsole" => LeftInsole,
        "rightInsole" => RightInsole,
        _ => null
    };

    public static BS_DiscoveredDeviceJson? Parse(in byte[] data)
    {
        Logger.Log($"parsing json ({data.Length} bytes)...");
        var jsonString = BS_StringUtils.GetString(data, true);

        if (jsonString == null)
        {
            Logger.LogError($"failed to parse json string");
            return null;
        }
        Logger.Log($"parsing json string {jsonString}");
        try
        {
            var discoveredDeviceJson = JsonUtility.FromJson<BS_DiscoveredDeviceJson>(jsonString);
            Logger.Log($"successfully parsed discoveredDevice {discoveredDeviceJson}");
            return discoveredDeviceJson;
        }
        catch (Exception e)
        {
            Logger.LogError($"JSON parsing failed: {e.Message}");
            return null;
        }
    }

    public override readonly string ToString()
    {
        return $"bluetoothId: {bluetoothId}, name: {name}, deviceType: {DeviceType}, rssi: {rssi}";
    }
}

