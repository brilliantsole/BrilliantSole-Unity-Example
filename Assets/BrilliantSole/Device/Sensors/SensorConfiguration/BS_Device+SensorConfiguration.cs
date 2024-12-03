using System;

using BS_SensorConfiguration = System.Collections.Generic.IReadOnlyDictionary<BS_SensorType, BS_SensorRate>;

public partial class BS_Device
{
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();

    public event Action<BS_Device, BS_SensorConfiguration> OnSensorConfiguration;

    private void SetupSensorConfigurationManager()
    {
        Managers.Add(SensorConfigurationManager);

        SensorConfigurationManager.OnSensorConfiguration = (BS_SensorConfiguration sensorRates) => OnSensorConfiguration?.Invoke(this, sensorRates);
    }
}
