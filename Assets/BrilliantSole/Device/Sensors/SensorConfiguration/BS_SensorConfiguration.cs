using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BS_SensorConfiguration
{
    private readonly Dictionary<BS_SensorType, BS_SensorRate> sensorRates = new();
    public IReadOnlyDictionary<BS_SensorType, BS_SensorRate> SensorRates => sensorRates;
}
