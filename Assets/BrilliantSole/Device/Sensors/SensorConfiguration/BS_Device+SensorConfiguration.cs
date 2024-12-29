using System;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public partial class BS_Device
{
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();

    public event Action<BS_Device, BS_SensorConfiguration> OnSensorConfiguration;

    private void SetupSensorConfigurationManager()
    {
        Managers.Add(SensorConfigurationManager);

        SensorConfigurationManager.OnSensorConfiguration = (sensorRates) => OnSensorConfiguration?.Invoke(this, sensorRates);
    }

    public void SetSensorConfiguration(BS_SensorConfiguration sensorConfiguration, bool clearRest = false, bool sendImmediately = true) { SensorConfigurationManager.SetSensorConfiguration(sensorConfiguration, clearRest, sendImmediately); }
    public void SetSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate, bool sendImmediately = true) { SensorConfigurationManager.SetSensorRate(sensorType, sensorRate, sendImmediately); }
    public void ToggleSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate, bool sendImmediately = true) { SensorConfigurationManager.ToggleSensorRate(sensorType, sensorRate, sendImmediately); }
    public void ClearSensorRate(BS_SensorType sensorType, bool sendImmediately = true) { SensorConfigurationManager.ClearSensorRate(sensorType, sendImmediately); }
    public void ClearSensorConfiguration(bool sendImmediately = true) { SensorConfigurationManager.ClearSensorConfiguration(sendImmediately); }
}
