public partial class BS_DevicePair
{
    private void AddDeviceListeners(BS_Device device)
    {
        AddDeviceConnectionListeners(device);
        AddDeviceSensorConfigurationListeners(device);
        AddDeviceSensorDataListeners(device);
        // FILL
    }
    private void RemoveDeviceListeners(BS_Device device)
    {
        RemoveDeviceConnectionListeners(device);
        RemoveDeviceSensorConfigurationListeners(device);
        RemoveDeviceSensorDataListeners(device);
        // FILL
    }
}
