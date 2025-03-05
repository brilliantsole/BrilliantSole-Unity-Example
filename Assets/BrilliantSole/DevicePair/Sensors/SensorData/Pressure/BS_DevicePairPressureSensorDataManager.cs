using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BS_DevicePairPressureSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePairPressureSensorDataManager");

    private readonly Dictionary<BS_InsoleSide, BS_PressureData> devicePressureData = new();
    private bool HasAllData => devicePressureData.Count == 2;
    private readonly BS_CenterOfPressureRange centerOfPressureRange = new();
    private readonly BS_Range normalizedSumRange = new();

    public Action<BS_DevicePairPressureData, ulong> OnPressureData;

    public void OnDevicePressureData(BS_InsoleSide insoleSide, BS_PressureData pressureData, ulong timestamp)
    {
        Logger.Log($"assigning {insoleSide} pressure data");
        devicePressureData[insoleSide] = pressureData;
        if (!HasAllData)
        {
            Logger.Log("doesn't have all pressure data");
            return;
        }
        Logger.Log("calculating devicePair pressure data");

        float scaledSum = 0.0f;
        float normalizedSum = 0.0f;
        foreach (var _insoleSide in devicePressureData.Keys)
        {
            scaledSum += devicePressureData[_insoleSide].ScaledSum;
        }
        normalizedSum = normalizedSumRange.UpdateAndGetNormalization(scaledSum, false);
        Logger.Log($"scaledSum: {scaledSum}, normalizedSum: {normalizedSum}");

        Dictionary<BS_InsoleSide, BS_PressureSensorData[]> sensors = new();
        foreach (var _insoleSide in devicePressureData.Keys)
        {
            sensors[_insoleSide] = new BS_PressureSensorData[devicePressureData[_insoleSide].Sensors.Count()];
        }
        Vector2? centerOfPressure = null;
        Vector2? normalizedCenterOfPressure = null;
        if (normalizedSum > 0.0f)
        {
            float centerOfPressureX = 0.0f;
            float centerOfPressureY = 0.0f;
            foreach (var _insoleSide in devicePressureData.Keys)
            {
                if (true)
                {
                    var numberOfPressureSensors = devicePressureData[_insoleSide].Sensors.Count();
                    for (int i = 0; i < numberOfPressureSensors; i++)
                    {
                        sensors[_insoleSide][i] = devicePressureData[_insoleSide].Sensors[i];
                        sensors[_insoleSide][i].WeightedValue = sensors[_insoleSide][i].ScaledValue / scaledSum;
                        sensors[_insoleSide][i].Position.x /= 2.0f;
                        if (_insoleSide == BS_InsoleSide.Right)
                        {
                            sensors[_insoleSide][i].Position.x += 0.5f;
                        }
                        centerOfPressureX += sensors[_insoleSide][i].Position.x * sensors[_insoleSide][i].WeightedValue;
                        centerOfPressureY += sensors[_insoleSide][i].Position.y * sensors[_insoleSide][i].WeightedValue;
                    }
                }
                else
                {
                    float normalizedSumWeight = devicePressureData[_insoleSide].NormalizedSum / normalizedSum;
                    if (normalizedSumWeight > 0.0f && devicePressureData[_insoleSide].NormalizedCenterOfPressure != null)
                    {
                        centerOfPressureY += (float)(devicePressureData[_insoleSide].NormalizedCenterOfPressure?.y) * normalizedSumWeight;
                        if (_insoleSide == BS_InsoleSide.Right)
                        {
                            centerOfPressureX = normalizedSumWeight;
                        }
                    }
                }

            }
            centerOfPressure = new(centerOfPressureX, centerOfPressureY);
            normalizedCenterOfPressure = centerOfPressureRange.UpdateAndGetNormalization((Vector2)centerOfPressure);

            Logger.Log($"centerOfPressure: {centerOfPressure}, normalizedCenterOfPressure: {normalizedCenterOfPressure}");
        }
        BS_DevicePairPressureData PressureData = new(sensors, scaledSum, normalizedSum, centerOfPressure, normalizedCenterOfPressure);
        OnPressureData?.Invoke(PressureData, timestamp);
    }
    public void Reset()
    {
        centerOfPressureRange.Reset();
        normalizedSumRange.Reset();
    }
}
