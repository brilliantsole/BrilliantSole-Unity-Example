using System.Collections.Generic;
using UnityEngine;

public class BS_DevicePairPressureSensorDataManager
{
    private readonly Dictionary<BS_InsoleSide, BS_PressureData> devicePressureData = new();
    private bool hasAllData => devicePressureData.Count == 2;
    private readonly BS_CenterOfPressureRange centerOfPressureRange = new();


    public void OnDevicePressureData(BS_InsoleSide insoleSide, BS_PressureData pressureData, ulong timestamp)
    {
        devicePressureData[insoleSide] = pressureData;
        if (!hasAllData) { return; }
        // FILL
    }
    public void Reset()
    {
        centerOfPressureRange.Reset();
    }
}
