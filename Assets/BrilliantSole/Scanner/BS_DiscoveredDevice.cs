using System;
using System.Text;
using UnityEngine;

#nullable enable

[System.Serializable]
public class BS_DiscoveredDevice
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDevice");

    public readonly string Id;
    public string Name { get; private set; }
    public BS_DeviceType? DeviceType { get; private set; }
    public int? Rssi { get; private set; }

    private DateTime LastTimeUpdated;

    public BS_DiscoveredDevice(string id, string name, BS_DeviceType? deviceType, int? rssi)
    {
        Name = name;
        DeviceType = deviceType;
        Id = id;
        Rssi = rssi;
        LastTimeUpdated = DateTime.Now;

        Logger.Log($"Created \"{DeviceType}\" with name \"{Name}\", rssi {Rssi}, and id {Id}");
    }
    public BS_DiscoveredDevice(BS_DiscoveredDeviceJson discoveredDeviceJson) : this(discoveredDeviceJson.bluetoothId, discoveredDeviceJson.name, discoveredDeviceJson.DeviceType, discoveredDeviceJson.rssi) { }

    public void Update(string? name, BS_DeviceType? deviceType, int? rssi)
    {
        if (name != null)
        {
            Name = name;
        }
        if (rssi != null)
        {
            Rssi = rssi;
        }
        if (deviceType != null)
        {
            DeviceType = deviceType;
        }
        LastTimeUpdated = DateTime.Now;
        Logger.Log($"Updated \"{Name}\"");
    }
    public void Update(BS_DiscoveredDeviceJson discoveredDeviceJson) { Update(discoveredDeviceJson.name, discoveredDeviceJson.DeviceType, discoveredDeviceJson.rssi); }

    // public static BS_DiscoveredDevice? ParseJson(in byte[] data)
    // {
    //     var nullableDiscoveredDeviceJson = BS_DiscoveredDeviceJson.Parse(data);
    //     if (nullableDiscoveredDeviceJson == null)
    //     {
    //         Logger.LogError("failed to parse discoveredDevice");
    //         return null;
    //     }

    //     var discoveredDeviceJson = (BS_DiscoveredDeviceJson)nullableDiscoveredDeviceJson;
    //     return new(discoveredDeviceJson.bluetoothId, discoveredDeviceJson.name, discoveredDeviceJson.DeviceType, discoveredDeviceJson.rssi);
    // }


    public TimeSpan TimeSinceLastUpdate => DateTime.Now - LastTimeUpdated;
}
