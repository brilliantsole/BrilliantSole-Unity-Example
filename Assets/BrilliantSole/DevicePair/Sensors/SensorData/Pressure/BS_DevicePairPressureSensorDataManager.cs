using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BS_DevicePairPressureSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePairPressureSensorDataManager");

    private readonly Dictionary<BS_Side, BS_PressureData> devicePressureData = new();
    private bool HasAllData => devicePressureData.Count == 2;
    private readonly BS_CenterOfPressureRange centerOfPressureRange = new();
    private readonly BS_Range normalizedSumRange = new();

    public Action<BS_DevicePairPressureData, ulong> OnPressureData;

    public void OnDevicePressureData(BS_Side side, BS_PressureData pressureData, ulong timestamp)
    {
        Logger.Log($"assigning {side} pressure data");
        devicePressureData[side] = pressureData;
        if (!HasAllData)
        {
            Logger.Log("doesn't have all pressure data");
            return;
        }
        Logger.Log("calculating devicePair pressure data");

        float scaledSum = 0.0f;
        float normalizedSum = 0.0f;
        foreach (var _side in devicePressureData.Keys)
        {
            scaledSum += devicePressureData[_side].ScaledSum;
        }
        normalizedSum = normalizedSumRange.UpdateAndGetNormalization(scaledSum, false);
        Logger.Log($"scaledSum: {scaledSum}, normalizedSum: {normalizedSum}");

        Dictionary<BS_Side, BS_PressureSensorData[]> sensors = new();
        foreach (var _side in devicePressureData.Keys)
        {
            sensors[_side] = new BS_PressureSensorData[devicePressureData[_side].Sensors.Count()];
        }
        Vector2? centerOfPressure = null;
        Vector2? normalizedCenterOfPressure = null;
        if (normalizedSum > 0.0f)
        {
            float centerOfPressureX = 0.0f;
            float centerOfPressureY = 0.0f;
            foreach (var _side in devicePressureData.Keys)
            {
                if (true)
                {
                    var numberOfPressureSensors = devicePressureData[_side].Sensors.Count();
                    for (int i = 0; i < numberOfPressureSensors; i++)
                    {
                        sensors[_side][i] = devicePressureData[_side].Sensors[i];
                        sensors[_side][i].WeightedValue = sensors[_side][i].ScaledValue / scaledSum;
                        sensors[_side][i].Position.x /= 2.0f;
                        if (_side == BS_Side.Right)
                        {
                            sensors[_side][i].Position.x += 0.5f;
                        }
                        centerOfPressureX += sensors[_side][i].Position.x * sensors[_side][i].WeightedValue;
                        centerOfPressureY += sensors[_side][i].Position.y * sensors[_side][i].WeightedValue;
                    }
                }
                else
                {
                    float normalizedSumWeight = devicePressureData[_side].NormalizedSum / normalizedSum;
                    if (normalizedSumWeight > 0.0f && devicePressureData[_side].NormalizedCenterOfPressure != null)
                    {
                        centerOfPressureY += (float)(devicePressureData[_side].NormalizedCenterOfPressure?.y) * normalizedSumWeight;
                        if (_side == BS_Side.Right)
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
