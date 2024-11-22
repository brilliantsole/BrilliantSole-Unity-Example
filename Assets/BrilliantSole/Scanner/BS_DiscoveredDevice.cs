using System;
using UnityEngine;

[System.Serializable]
public struct BS_DiscoveredDevice
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDevice", BS_Logger.LogLevel.Log);

    public readonly string Name;
    public readonly BS_DeviceType DeviceType;
    public int Rssi { get; private set; }
    public readonly string Id;

    private readonly DateTime CreationDate;

    public BS_DiscoveredDevice(string id, string name, BS_DeviceType deviceType, int rssi)
    {
        Name = name;
        DeviceType = deviceType;
        Id = id;
        Rssi = rssi;
        CreationDate = DateTime.Now;

        Logger.Log($"Created \"{DeviceType}\" with name \"{Name}\", rssi {Rssi}, and id {Id}");
    }

    public readonly TimeSpan TimeSinceCreation => DateTime.Now - CreationDate;
}
