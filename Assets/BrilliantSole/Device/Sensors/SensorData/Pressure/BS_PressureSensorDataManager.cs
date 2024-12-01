using System;
using System.Collections.Generic;
using UnityEngine;
using static BS_SensorType;

public class BS_PressureSensorDataManager : BS_BaseSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_PressureSensorDataManager", BS_Logger.LogLevel.Log);

    private readonly HashSet<BS_SensorType> sensorTypes = new() { Pressure };
    protected override HashSet<BS_SensorType> SensorTypes => sensorTypes;

    public override void ParseSensorDataMessage(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
    {
        base.ParseSensorDataMessage(sensorType, data, timestamp, scalar);
        switch (sensorType)
        {
            case Pressure:
                ParsePressureData(data, timestamp, scalar);
                break;
            default:
                throw new ArgumentException($"uncaught sensorType {sensorType}");
        }
    }

    private readonly List<Vector2> pressurePositions = new();
    public IList<Vector2> PressurePositions => pressurePositions;
    public int NumberOfPressureSensors => PressurePositions.Count;
    public void ParsePressurePositions(in byte[] data)
    {
        pressurePositions.Clear();

        for (int i = 0; i < data.Length; i += 2)
        {
            float x = data[i];
            float y = data[i + 1];
            Vector2 pressurePosition = new(x, y);
            pressurePosition /= PressurePositionScalar;
            Logger.Log($"pressure position #{pressurePositions.Count}: [{pressurePosition.x}, {pressurePosition.y}]");
            pressurePositions.Add(pressurePosition);
        }
        Logger.Log($"Parsed {NumberOfPressureSensors} Pressure positions");
    }

    static readonly float PressurePositionScalar = Mathf.Pow(2, 8);
    private void ParsePressureData(in byte[] data, in ulong timestamp, in float scalar)
    {
        // FILL
    }

    public override void Reset()
    {
        base.Reset();
        pressurePositions.Clear();
    }
}
