using System;
using System.Collections.Generic;
using static BS_SensorDataMessageType;

public class BS_SensorDataManager : BS_BaseManager<BS_SensorDataMessageType>
{
    public static readonly BS_SensorDataMessageType[] RequiredMessageTypes = {
        GetPressurePositions,
        GetSensorScalars
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    public readonly BS_PressureSensorDataManager PressureSensorDataManager = new();
    public readonly BS_MotionSensorDataManager MotionSensorDataManager = new();
    public readonly BS_BarometerSensorDataManager BarometerSensorDataManager = new();

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SensorDataManager", BS_Logger.LogLevel.Warn);

    public override void OnRxMessage(BS_SensorDataMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetPressurePositions:
                PressureSensorDataManager.ParsePressurePositions(data);
                break;
            case GetSensorScalars:
                ParseSensorScalars(data);
                break;
            case SensorData:
                ParseSensorData(data);
                break;
        }
    }

    static public void AssertValidSensorTypeEnum(byte sensorTypeEnum)
    {
        if (!Enum.IsDefined(typeof(BS_SensorType), sensorTypeEnum))
        {
            throw new ArgumentException($"Invalid BS_SensorType {sensorTypeEnum}");
        }
    }

    private readonly Dictionary<BS_SensorType, float> sensorScalars = new();
    public IReadOnlyDictionary<BS_SensorType, float> SensorScalars => sensorScalars;

    private void ParseSensorScalars(in byte[] data)
    {
        sensorScalars.Clear();

        byte stride = sizeof(float) + 1;
        for (int i = 0; i < data.Length; i += stride)
        {
            var sensorTypeEnum = data[i];
            //Logger.Log($"sensorTypeEnum: {sensorTypeEnum}");
            AssertValidSensorTypeEnum(sensorTypeEnum);
            var sensorType = (BS_SensorType)sensorTypeEnum;
            //Logger.Log($"sensorType: {sensorType}");

            var sensorScalar = BS_ByteUtils.ParseNumber<float>(data, i + 1, true);
            Logger.Log($"{sensorType} scalar: {sensorScalar}");

            sensorScalars.Add(sensorType, sensorScalar);
        }

        Logger.Log("Parsed sensor scalars");
    }

    private void ParseSensorData(in byte[] data)
    {
        ushort offset = 0;
        var timestamp = BS_TimeUtils.ParseTimestamp(data, offset);
        offset += 2;
        Logger.Log($"timestamp: {timestamp}ms");

        BS_ParseUtils.ParseMessages(data, (byte sensorTypeEnum, byte[] data) => OnSensorDataMessage(sensorTypeEnum, data, timestamp), offset, false);
    }

    private void OnSensorDataMessage(byte sensorTypeEnum, in byte[] data, in ulong timestamp)
    {
        AssertValidSensorTypeEnum(sensorTypeEnum);
        var sensorType = (BS_SensorType)sensorTypeEnum;
        Logger.Log($"sensorType: {sensorType}");

        var scalar = sensorScalars.GetValueOrDefault(sensorType, 1.0f);
        Logger.Log($"scalar: {scalar}");

        foreach (var SensorDataManager in SensorDataManagers)
        {
            if (SensorDataManager.CanParseSensorDataMessage(sensorType))
            {
                SensorDataManager.ParseSensorDataMessage(sensorType, data, timestamp, scalar);
                break;
            }
        }

    }

    private readonly BS_BaseSensorDataManager[] SensorDataManagers;

    public override void Reset()
    {
        base.Reset();

        foreach (var SensorDataManager in SensorDataManagers) { SensorDataManager.Reset(); }
    }

    public BS_SensorDataManager()
    {
        SensorDataManagers = new BS_BaseSensorDataManager[] { PressureSensorDataManager, MotionSensorDataManager, BarometerSensorDataManager };
    }
}
