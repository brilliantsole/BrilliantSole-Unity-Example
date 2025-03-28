using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static BS_SensorType;

public class BS_PressureSensorDataManager : BS_BaseSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_PressureSensorDataManager");

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
    public ReadOnlyCollection<Vector2> PressurePositions => pressurePositions.AsReadOnly();
    public int NumberOfPressureSensors => PressurePositions.Count;
    static readonly float PressurePositionScalar = Mathf.Pow(2, 8);
    public void ParsePressurePositions(in byte[] data)
    {
        pressurePositions.Clear();
        pressureSensorRanges.Clear();

        for (int i = 0; i < data.Length; i += 2)
        {
            float x = data[i];
            float y = data[i + 1];
            Vector2 pressurePosition = new(x, y);
            pressurePosition /= PressurePositionScalar;
            Logger.Log($"pressure position #{pressurePositions.Count}: [{pressurePosition.x}, {pressurePosition.y}]");
            pressurePositions.Add(pressurePosition);

            pressureSensorRanges.Add(new());
        }
        Logger.Log($"Parsed {NumberOfPressureSensors} Pressure positions");
    }

    private readonly List<BS_Range> pressureSensorRanges = new();
    private readonly BS_CenterOfPressureRange centerOfPressureRange = new();
    private readonly BS_Range normalizedSumRange = new();

    public Action<BS_PressureData, ulong> OnPressureData;
    private void ParsePressureData(in byte[] data, in ulong timestamp, in float scalar)
    {
        var expectedDataLength = NumberOfPressureSensors * 2;
        if (data.Length != expectedDataLength)
        {
            throw new ArgumentException($"invalid number of pressure sensors (expected {expectedDataLength}, got {data.Length})");
        }

        BS_PressureSensorData[] Sensors = new BS_PressureSensorData[NumberOfPressureSensors];
        float ScaledSum = 0.0f;
        float NormalizedSum = 0.0f;

        for (int i = 0; i < NumberOfPressureSensors; i++)
        {
            var RawValue = BS_ByteUtils.ParseNumber<ushort>(data, i * 2, true);
            Logger.Log($"pressure #{i} RawValue: {RawValue}");

            var ScaledValue = RawValue * scalar / NumberOfPressureSensors;
            Logger.Log($"pressure #{i} ScaledValue: {RawValue}");

            var NormalizedValue = pressureSensorRanges[i].UpdateAndGetNormalization(ScaledValue, false);
            Logger.Log($"pressure #{i} NormalizedValue: {NormalizedValue}");

            float WeightedValue = 0.0f;

            Sensors[i] = new(PressurePositions[i], RawValue, ScaledValue, NormalizedValue, WeightedValue);
            Logger.Log($"PressureSensor #{i}: {Sensors[i]}");

            ScaledSum += ScaledValue;
            //NormalizedSum += NormalizedValue;
            Logger.Log($"partial (#{i}) ScaledSum: {ScaledSum}");
        }
        NormalizedSum = normalizedSumRange.UpdateAndGetNormalization(ScaledSum, false);
        Logger.Log($"final ScaledSum: {ScaledSum}, NormalizedSum: {NormalizedSum}");

        Vector2? CenterOfPressure = null;
        Vector2? NormalizedCenterOfPressure = null;

        if (ScaledSum > 0)
        {
            CenterOfPressure = new();
            foreach (ref var Sensor in Sensors.AsSpan())
            {
                Sensor.WeightedValue = Sensor.ScaledValue / ScaledSum;
                CenterOfPressure += Sensor.Position * Sensor.WeightedValue;
            }
            Logger.Log($"CenterOfPressure: {CenterOfPressure}");

            NormalizedCenterOfPressure = centerOfPressureRange.UpdateAndGetNormalization((Vector2)CenterOfPressure);
            Logger.Log($"NormalizedCenterOfPressure: {NormalizedCenterOfPressure}");
        }
        else
        {
            Logger.Log("ScaledSum is 0 - skipping CenterOfPressure calculation");
        }

        BS_PressureData PressureData = new(Sensors, ScaledSum, NormalizedSum, CenterOfPressure, NormalizedCenterOfPressure);
        OnPressureData?.Invoke(PressureData, timestamp);
    }

    public override void Reset()
    {
        base.Reset();
        centerOfPressureRange.Reset();
        normalizedSumRange.Reset();
        foreach (var PressureSensorRange in pressureSensorRanges) { PressureSensorRange.Reset(); }
    }
}
