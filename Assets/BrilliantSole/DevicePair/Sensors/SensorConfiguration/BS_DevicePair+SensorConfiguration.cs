using System;
using UnityEngine;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public partial class BS_DevicePair
{
    private void AddDeviceSensorConfigurationListeners(BS_Device device)
    {
        device.OnSensorConfiguration += onDeviceSensorConfiguration;
    }
    private void RemoveDeviceSensorConfigurationListeners(BS_Device device)
    {
        device.OnSensorConfiguration -= onDeviceSensorConfiguration;
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, BS_SensorConfiguration> OnDeviceSensorConfiguration;
    private void onDeviceSensorConfiguration(BS_Device device, BS_SensorConfiguration sensorConfiguration)
    {
        OnDeviceSensorConfiguration?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, sensorConfiguration);
    }

    public void SetSensorConfiguration(BS_SensorConfiguration sensorConfiguration, bool clearRest = false)
    {
        foreach (var device in devices.Values) { device.SetSensorConfiguration(sensorConfiguration, clearRest); }
    }
    public void SetSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate)
    {
        foreach (var device in devices.Values) { device.SetSensorRate(sensorType, sensorRate); }
    }
    public void ClearSensorRate(BS_SensorType sensorType)
    {
        foreach (var device in devices.Values) { device.ClearSensorRate(sensorType); }
    }
    public void ToggleSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate)
    {
        foreach (var device in devices.Values) { device.ToggleSensorRate(sensorType, sensorRate); }
    }
    public void ClearSensorConfiguration()
    {
        foreach (var device in devices.Values) { device.ClearSensorConfiguration(); }
    }
}
