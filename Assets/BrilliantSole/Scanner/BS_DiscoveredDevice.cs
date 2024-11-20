using System;
using UnityEngine;

public struct BS_DiscoveredDevice
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDevice", BS_Logger.LogLevel.Log);

    public readonly string Name;
    public int Rssi { get; private set; }
    public readonly string Id;

    public DateTime LastRssiUpdate { get; private set; }

    public BS_DiscoveredDevice(string name, int rssi, string id)
    {
        Name = name;
        Rssi = rssi;
        Id = id;
        LastRssiUpdate = DateTime.Now;

        Logger.Log($"Created device with name \"{Name}\", rssi {Rssi}, and id {Id}");
    }

    public void UpdateRssi(int newRssi)
    {
        Logger.Log($"Updating Rssi to {newRssi}");
        Rssi = newRssi;
        LastRssiUpdate = DateTime.Now;
    }

    public readonly TimeSpan TimeSinceLastRssiUpdate => DateTime.Now - LastRssiUpdate;
}
