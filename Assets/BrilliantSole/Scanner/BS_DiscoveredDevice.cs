using System;

#nullable enable

[Serializable]
public class BS_DiscoveredDevice
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DiscoveredDevice");

    public readonly string Id;
    public string Name { get; private set; }
    public BS_DeviceType? DeviceType { get; private set; }
    public int? Rssi { get; private set; }

    private DateTime LastTimeUpdated;

    public BS_DiscoveredDevice(IBS_Scanner scanner, string id, string name, BS_DeviceType? deviceType, int? rssi)
    {
        Scanner = scanner;
        Name = name;
        DeviceType = deviceType;
        Id = id;
        Rssi = rssi;
        LastTimeUpdated = DateTime.Now;

        Logger.Log($"Created \"{DeviceType}\" with name \"{Name}\", rssi {Rssi}, and id {Id}");
    }
    public BS_DiscoveredDevice(IBS_Scanner scanner, BS_DiscoveredDeviceJson discoveredDeviceJson) : this(scanner, discoveredDeviceJson.bluetoothId, discoveredDeviceJson.name, discoveredDeviceJson.DeviceType, discoveredDeviceJson.rssi) { }

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

    public TimeSpan TimeSinceLastUpdate => DateTime.Now - LastTimeUpdated;

    public readonly IBS_Scanner Scanner;
    public BS_Device Connect() { return Scanner.ConnectToDiscoveredDevice(this); }
    public BS_Device? Disconnect() { return Scanner.DisconnectFromDiscoveredDevice(this); }
    public BS_Device ToggleConnection() { return Scanner.ToggleConnectionToDiscoveredDevice(this); }

    public BS_Device? Device => Scanner.Devices.ContainsKey(Id) ? Scanner.Devices[Id] : null;
}
