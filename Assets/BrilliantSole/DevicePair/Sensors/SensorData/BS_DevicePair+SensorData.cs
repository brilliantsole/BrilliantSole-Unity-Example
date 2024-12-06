using System;

public partial class BS_DevicePair
{
    private readonly BS_DevicePairSensorDataManager SensorDataManager = new();

    public event Action<BS_DevicePair, BS_DevicePairPressureData, ulong> OnPressureData;

    private void SetupSensorDataManager()
    {
        SensorDataManager.PressureSensorDataManager.OnPressureData = (BS_DevicePairPressureData pressureData, ulong timestamp) => OnPressureData?.Invoke(this, pressureData, timestamp);
    }
}
