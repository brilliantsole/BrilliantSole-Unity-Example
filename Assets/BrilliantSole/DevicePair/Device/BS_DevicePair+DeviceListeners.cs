public partial class BS_DevicePair
{
    private void AddDeviceListeners(BS_Device device)
    {
        AddDeviceConnectionListeners(device);
        AddDeviceSensorConfigurationListeners(device);
        AddDeviceSensorDataListeners(device);
        AddDeviceVibrationListeners(device);
        AddDeviceFileTransferListeners(device);
        AddDeviceTfliteListeners(device);
    }
    private void RemoveDeviceListeners(BS_Device device)
    {
        RemoveDeviceConnectionListeners(device);
        RemoveDeviceSensorConfigurationListeners(device);
        RemoveDeviceSensorDataListeners(device);
        RemoveDeviceVibrationListeners(device);
        RemoveDeviceFileTransferListeners(device);
        RemoveDeviceTfliteListeners(device);
    }
}
