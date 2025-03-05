using System;
using System.Collections.Generic;

public abstract class BS_BaseSensorDataManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseSensorDataManager");

    protected abstract HashSet<BS_SensorType> SensorTypes { get; }

    public virtual bool CanParseSensorDataMessage(BS_SensorType sensorType) { return SensorTypes.Contains(sensorType); }
    private void AssertValidMessageTypeEnum(BS_SensorType sensorTypeEnum)
    {
        if (!CanParseSensorDataMessage(sensorTypeEnum))
        {
            throw new ArgumentException($"Invalid sensorType {sensorTypeEnum}");
        }
    }
    public virtual void ParseSensorDataMessage(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
    {
        AssertValidMessageTypeEnum(sensorType);
        Logger.Log($"parsing sensorType {sensorType} ({data.Length} bytes, {scalar} scalar, {timestamp} timestamp)");
    }

    public virtual void Reset()
    {
        Logger.Log("Resetting...");
    }
}
