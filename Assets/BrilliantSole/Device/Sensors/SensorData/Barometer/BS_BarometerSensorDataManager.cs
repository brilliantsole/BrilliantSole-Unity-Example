using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using static BS_SensorType;

public class BS_BarometerSensorDataManager : BS_BaseSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BarometerSensorDataManager");

    private readonly HashSet<BS_SensorType> sensorTypes = new() { Barometer };
    protected override HashSet<BS_SensorType> SensorTypes => sensorTypes;

    public override void ParseSensorDataMessage(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
    {
        base.ParseSensorDataMessage(sensorType, data, timestamp, scalar);
        switch (sensorType)
        {
            case Barometer:
                ParseBarometer(data, timestamp, scalar);
                break;
            default:
                throw new ArgumentException($"uncaught sensorType {sensorType}");
        }
    }

    public Action<float, ulong> OnBarometer;

    private void ParseBarometer(in byte[] data, in ulong timestamp, float scalar)
    {
        var barometer = BS_ByteUtils.ParseNumber<float>(data, 0, true);
        barometer *= scalar;
        Logger.Log($"barometer: {barometer}");
        OnBarometer?.Invoke(barometer, timestamp);
    }
}
