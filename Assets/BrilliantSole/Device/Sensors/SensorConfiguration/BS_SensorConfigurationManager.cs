using System;
using System.Collections.Generic;
using static BS_SensorConfigurationMessageType;
using static BS_SensorRate;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public class BS_SensorConfigurationManager : BS_BaseManager<BS_SensorConfigurationMessageType>
{
    public static readonly BS_SensorConfigurationMessageType[] RequiredMessageTypes = {
        GetSensorConfiguration
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SensorConfigurationManager", BS_Logger.LogLevel.Warn);

    public override void OnRxMessage(BS_SensorConfigurationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case BS_SensorConfigurationMessageType.GetSensorConfiguration:
            case BS_SensorConfigurationMessageType.SetSensorConfiguration:
                ParseSensorConfiguration(data);
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    private BS_SensorConfiguration TempSensorConfiguration = new();
    public BS_SensorConfiguration SensorConfiguration { get; private set; } = new();
    public ICollection<BS_SensorType> SensorTypes => SensorConfiguration.Keys;

    public Action<BS_SensorConfiguration> OnSensorConfiguration;
    private void ParseSensorConfiguration(in byte[] data)
    {
        BS_SensorConfiguration ParsedSensorConfiguration = new();

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
            ParsedSensorConfiguration.Add(sensorType, sensorRate);
        }

        SensorConfiguration = ParsedSensorConfiguration;
        Logger.Log($"updated sensorConfiguration:\n{PrintSensorConfiguration()}");
        OnSensorConfiguration?.Invoke(SensorConfiguration);
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

    public bool ContainsSensorType(BS_SensorType sensorType) { return SensorTypes.Contains(sensorType); }
    public bool IsSensorRateNonZero(BS_SensorType sensorType) { return SensorConfiguration.GetValueOrDefault(sensorType, _0ms) != _0ms; }
    public BS_SensorRate? GetSensorRate(BS_SensorType sensorType) { return ContainsSensorType(sensorType) ? SensorConfiguration[sensorType] : null; }

    public void SetSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate)
    {
        if (!ContainsSensorType(sensorType))
        {
            Logger.LogError($"configuration doesn't contain sensorType {sensorType}");
            return;
        }
        if (GetSensorRate(sensorType) == sensorRate)
        {
            Logger.LogError($"redundant sensorRate {sensorRate} for sensorType {sensorType}");
            return;
        }

        TempSensorConfiguration.Clear();
        TempSensorConfiguration.Add(sensorType, sensorRate);
        SetSensorConfiguration(TempSensorConfiguration);
    }
    public void ToggleSensorRate(BS_SensorType sensorType, BS_SensorRate sensorRate) { SetSensorRate(sensorType, IsSensorRateNonZero(sensorType) ? _0ms : sensorRate); }
    public void ClearSensorRate(BS_SensorType sensorType) { SetSensorRate(sensorType, _0ms); }

    private readonly List<byte> Array = new();
    private List<byte> GetSensorConfigurationArray(in BS_SensorConfiguration sensorConfiguration)
    {
        Array.Clear();
        foreach (var pair in sensorConfiguration)
        {
            Array.Add((byte)pair.Key);
            Array.AddRange(BS_ByteUtils.ToByteArray((ushort)pair.Value, true));
        }
        return Array;
    }

    public void SetSensorConfiguration(in BS_SensorConfiguration sensorConfiguration, bool clearRest = false, bool sendImmediately = true)
    {
        if (AreSensorConfigurationsEqual(SensorConfiguration, sensorConfiguration, false))
        {
            Logger.Log("sensorConfigurations are equal - no need to set");
            return;
        }

        TempSensorConfiguration = sensorConfiguration;
        if (clearRest)
        {
            foreach (var sensorType in SensorTypes)
            {
                if (!TempSensorConfiguration.ContainsKey(sensorType)) { TempSensorConfiguration.Add(sensorType, _0ms); }
            }
        }

        BS_TxMessage[] Messages = { CreateTxMessage(BS_SensorConfigurationMessageType.SetSensorConfiguration, GetSensorConfigurationArray(TempSensorConfiguration)) };
        SendTxMessages(Messages, sendImmediately);
    }

    private bool AreSensorConfigurationsEqual(in BS_SensorConfiguration a, in BS_SensorConfiguration b, bool areStrictlyEqual = true)
    {
        if (a.Count != b.Count)
        {
            if (areStrictlyEqual) { return false; }
            else if (a.Count < b.Count) { return AreSensorConfigurationsEqual(b, a, areStrictlyEqual); }
        }

        foreach (var pair in a)
        {
            if (!b.ContainsKey(pair.Key))
            {
                if (areStrictlyEqual) { return false; }
            }
            else
            {
                if (b[pair.Key] != pair.Value) { return false; }
            }
        }
        return true;
    }

    public override void Reset()
    {
        base.Reset();
        SensorConfiguration.Clear();
        TempSensorConfiguration.Clear();
    }

    public void ClearSensorConfiguration()
    {
        TempSensorConfiguration.Clear();
        foreach (var sensorType in SensorTypes) { TempSensorConfiguration.Add(sensorType, _0ms); }
        SetSensorConfiguration(TempSensorConfiguration);
    }

    public string PrintSensorConfiguration()
    {
        string _string = "";
        foreach (var pair in SensorConfiguration) { _string += $"{pair.Key}: {pair.Value}, "; }
        return _string;
    }
}
