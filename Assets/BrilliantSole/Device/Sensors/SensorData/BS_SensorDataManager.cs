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

    private readonly BS_PressureSensorDataManager PressureSensorDataManager = new();
    private readonly BS_MotionSensorDataManager MotionSensorDataManager = new();
    private readonly BS_BarometerSensorDataManager BarometerSensorDataManager = new();

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SensorDataManager", BS_Logger.LogLevel.Log);

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

            float sensorScalar = BS_ByteUtils.ParseNumber<float>(data, i + 1, true);
            Logger.Log($"{sensorType} scalar: {sensorScalar}");

            sensorScalars.Add(sensorType, sensorScalar);
        }

        Logger.Log("Parsed sensor scalars");
    }

    private void ParseSensorData(in byte[] data)
    {
        // https://github.com/brilliantsole/Brilliant-Sole-Unreal/blob/9abf5b05670009c965f9e648ca73f2a270be8d5d/Plugins/BrilliantSoleSDK/Source/BrilliantSoleSDK/Private/BS_SensorDataManager.cpp#L76
    }

    public override void Reset()
    {
        base.Reset();

        // FILL
    }
}
