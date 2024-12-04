using System;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public partial class BS_Device
{
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();

    public event Action<BS_Device, BS_SensorConfiguration> OnSensorConfiguration;

    private void SetupSensorConfigurationManager()
    {
        Managers.Add(SensorConfigurationManager);

        SensorConfigurationManager.OnSensorConfiguration = (BS_SensorConfiguration sensorRates) => OnSensorConfiguration?.Invoke(this, sensorRates);
    }

    public void SetSensorConfiguration(BS_SensorConfiguration sensorConfiguration, bool clearRest = false) { SensorConfigurationManager.SetSensorConfiguration(sensorConfiguration, clearRest); }
    public void SetSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate) { SensorConfigurationManager.SetSensorRate(sensorType, sensorRate); }
    public void ToggleSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate) { SensorConfigurationManager.ToggleSensorRate(sensorType, sensorRate); }
    public void ClearSensorRate(BS_SensorType sensorType) { SensorConfigurationManager.ClearSensorRate(sensorType); }
    public void ClearSensorConfiguration() { SensorConfigurationManager.ClearSensorConfiguration(); }
}
