using System;
using System.Collections.Generic;
using UnityEngine;

public class BS_DevicePairPressureSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePairPressureSensorDataManager", BS_Logger.LogLevel.Log);

    private readonly Dictionary<BS_InsoleSide, BS_PressureData> devicePressureData = new();
    private bool HasAllData => devicePressureData.Count == 2;
    private readonly BS_CenterOfPressureRange centerOfPressureRange = new();

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

        float rawSum = 0.0f;
        float normalizedSum = 0.0f;
        foreach (var _insoleSide in devicePressureData.Keys)
        {
            rawSum += devicePressureData[_insoleSide].ScaledSum;
            normalizedSum += devicePressureData[_insoleSide].NormalizedSum;
        }

        Logger.Log($"rawSum: {rawSum}, normalizedSum: {normalizedSum}");

        Vector2? centerOfPressure = null;
        Vector2? normalizedCenterOfPressure = null;
        if (normalizedSum > 0.0f)
        {
            float centerOfPressureX = 0.0f;
            float centerOfPressureY = 0.0f;
            foreach (var _insoleSide in devicePressureData.Keys)
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
            centerOfPressure = new(centerOfPressureX, centerOfPressureY);
            normalizedCenterOfPressure = centerOfPressureRange.UpdateAndGetNormalization((Vector2)centerOfPressure);

            Logger.Log($"centerOfPressure: {centerOfPressure}, normalizedCenterOfPressure: {normalizedCenterOfPressure}");
        }

        BS_DevicePairPressureData PressureData = new(rawSum, normalizedSum, centerOfPressure, normalizedCenterOfPressure);
        OnPressureData?.Invoke(PressureData, timestamp);
    }
    public void Reset()
    {
        centerOfPressureRange.Reset();
    }
}
