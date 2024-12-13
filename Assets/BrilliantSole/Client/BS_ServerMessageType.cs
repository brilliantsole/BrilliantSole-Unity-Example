public enum BS_ServerMessageType : byte
{
    IsScanningAvailable,
    IsScanning,
    StartScan,
    StopScan,
    DiscoveredDevice,
    DiscoveredDevices,
    ExpiredDiscoveredDevice,
    ConnectToDevice,
    DisconnectFromDevice,
    ConnectedDevices,
    DeviceMessage
}