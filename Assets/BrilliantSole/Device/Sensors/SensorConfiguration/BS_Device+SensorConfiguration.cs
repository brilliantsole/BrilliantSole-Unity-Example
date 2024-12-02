using System;

using BS_SensorRates = System.Collections.Generic.IReadOnlyDictionary<BS_SensorType, BS_SensorRate>;

public partial class BS_Device
{
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();

    public event Action<BS_Device, BS_SensorRates> OnSensorRates;

    private void SetupSensorConfigurationManager()
    {
        Managers.Add(SensorConfigurationManager);

        SensorConfigurationManager.OnSensorRates = (BS_SensorRates sensorRates) => OnSensorRates?.Invoke(this, sensorRates);
    }
}
