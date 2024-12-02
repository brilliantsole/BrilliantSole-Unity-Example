using UnityEngine;
using System;
using System.Collections.Generic;

using BS_SensorRates = System.Collections.Generic.IReadOnlyDictionary<BS_SensorType, BS_SensorRate>;

[Serializable]
public class BS_SensorConfiguration
{
    private readonly Dictionary<BS_SensorType, BS_SensorRate> sensorRates = new();
    public BS_SensorRates SensorRates => sensorRates;

    public void Clear()
    {
        sensorRates.Clear();
    }
}