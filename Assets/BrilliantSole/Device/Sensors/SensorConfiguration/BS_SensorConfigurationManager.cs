using System;
using static BS_SensorConfigurationMessageType;

using BS_SensorRates = System.Collections.Generic.IReadOnlyDictionary<BS_SensorType, BS_SensorRate>;

public class BS_SensorConfigurationManager : BS_BaseManager<BS_SensorConfigurationMessageType>
{
    public static readonly BS_SensorConfigurationMessageType[] RequiredMessageTypes = {
        GetSensorConfiguration
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SensorConfigurationManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_SensorConfigurationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetSensorConfiguration:
            case SetSensorConfiguration:
                ParseSensorConfiguration(data);
                break;
            default:
                throw new ArgumentException($"uncaught messageType {messageType}");
        }
    }

    private readonly BS_SensorConfiguration sensorConfiguration = new();
    public BS_SensorRates SensorRates => sensorConfiguration.SensorRates;

    public Action<BS_SensorRates> OnSensorRates;

    private void ParseSensorConfiguration(in byte[] data)
    {
        Logger.Log($"parsing sensor configuration ({data.Length} bytes)");
        sensorConfiguration.Parse(data);
        OnSensorRates?.Invoke(SensorRates);
    }

    public override void Reset()
    {
        base.Reset();
        sensorConfiguration.Clear();
    }
}
