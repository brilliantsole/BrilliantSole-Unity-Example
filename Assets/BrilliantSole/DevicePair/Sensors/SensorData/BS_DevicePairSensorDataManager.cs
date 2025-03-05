public class BS_DevicePairSensorDataManager
{
    public readonly BS_DevicePairPressureSensorDataManager PressureSensorDataManager = new();
    public readonly BS_DevicePairMotionSensorDataManager MotionSensorDataManager = new();
    public readonly BS_DevicePairBarometerSensorDataManager BarometerSensorDataManager = new();

    public void Reset()
    {
        PressureSensorDataManager.Reset();
    }
}
