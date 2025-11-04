using System;

public partial class BS_DevicePair
{
    private readonly BS_DevicePairSensorDataManager SensorDataManager = new();

    public delegate void OnPressureDataDelegate(
        BS_DevicePair devicePair,
        BS_DevicePairPressureData pressureData,
        ulong timestamp
    );
    public event OnPressureDataDelegate OnPressureData;

    private void SetupSensorDataManager()
    {
        SensorDataManager.PressureSensorDataManager.OnPressureData += (pressureData, timestamp) => OnPressureData?.Invoke(this, pressureData, timestamp);
    }
}
