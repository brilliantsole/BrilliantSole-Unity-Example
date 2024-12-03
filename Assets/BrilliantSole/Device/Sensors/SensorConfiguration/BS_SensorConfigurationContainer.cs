using System;
using System.Collections.Generic;
using static BS_SensorRate;
using System.Collections.ObjectModel;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public class BS_SensorConfigurationContainer
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SensorConfigurationContainer", BS_Logger.LogLevel.Log);

    private readonly Dictionary<BS_SensorType, BS_SensorRate> sensorRates = new();
    public BS_SensorConfiguration SensorRates => sensorRates;

    public ICollection<BS_SensorType> SensorTypes => sensorRates.Keys;

    public void Parse(in byte[] data)
    {
        sensorRates.Clear();

        Logger.Log("parsing sensorConfiguration");
        for (int i = 0; i < data.Length; i += 3)
        {
            var sensorTypeEnum = data[i];
            BS_SensorDataManager.AssertValidSensorTypeEnum(sensorTypeEnum);
            var sensorType = (BS_SensorType)sensorTypeEnum;
            Logger.Log($"sensorType: {sensorType}");

            var rawSensorRate = BS_ByteUtils.ParseNumber<ushort>(data, i + 1, true);
            Logger.Log($"rawSensorRate: {rawSensorRate}ms");

            var sensorRate = GetClosestSensorRate(rawSensorRate);
            Logger.Log($"sensorRate: {sensorRate}");
            rawSensorRate = (ushort)sensorRate;

            Logger.Log($"{sensorType}: {sensorRate} ({rawSensorRate}ms)");
            sensorRates.Add(sensorType, sensorRate);
        }

        Logger.Log($"updated sensorConfiguration:\n{this}");
        ArrayNeedsUpdate = true;
    }

    private BS_SensorRate GetClosestSensorRate(ushort rawSensorRate)
    {
        var sensorRate = _0ms;
        if (rawSensorRate > 0)
        {
            foreach (BS_SensorRate _sensorRate in Enum.GetValues(typeof(BS_SensorRate)))
            {
                var _rawSensorRate = (ushort)_sensorRate;
                if (rawSensorRate >= _rawSensorRate)
                {
                    sensorRate = _sensorRate;
                }
            }
        }
        return sensorRate;
    }

    public void Clear()
    {
        sensorRates.Clear();
        ArrayNeedsUpdate = true;
    }

    public override string ToString()
    {
        string _string = "";
        foreach (var pair in sensorRates) { _string += $"{pair.Key}: {pair.Value}, "; }
        return _string;
    }

    private bool ArrayNeedsUpdate = false;
    private readonly List<byte> Array = new();
    public ReadOnlyCollection<byte> ToArray()
    {
        if (ArrayNeedsUpdate)
        {
            ArrayNeedsUpdate = false;
            Array.Clear();
            foreach (var pair in SensorRates)
            {
                Array.Add((byte)pair.Key);
                Array.AddRange(BS_ByteUtils.ToByteArray((ushort)pair.Value, true));
            }
        }
        return Array.AsReadOnly();
    }
}