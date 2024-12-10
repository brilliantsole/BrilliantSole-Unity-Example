using System;
using System.Collections.Generic;
using UnityEngine;
using static BS_TfliteSensorTypeFlag;

[CreateAssetMenu(fileName = "NewModelMetadata", menuName = "Brilliant Sole/TfliteModelMetadata")]
public class BS_TfliteModelMetadata : BS_FileMetadata, ISerializationCallbackReceiver
{
    public override BS_FileType FileType => BS_FileType.Tflite;

    public string Name;
    public BS_TfliteSensorTypeFlag SensorTypes;
    public List<BS_SensorType> GetSensorTypes()
    {
        List<BS_SensorType> sensorTypes = new();
        foreach (BS_TfliteSensorTypeFlag value in Enum.GetValues(typeof(BS_TfliteSensorTypeFlag)))
        {
            if (SensorTypes.HasFlag(value) && value != None)
            {
                switch (value)
                {
                    case Pressure:
                        sensorTypes.Add(BS_SensorType.Pressure);
                        break;
                    case LinearAcceleration:
                        sensorTypes.Add(BS_SensorType.LinearAcceleration);
                        break;
                    case BS_TfliteSensorTypeFlag.Gyroscope:
                        sensorTypes.Add(BS_SensorType.Gyroscope);
                        break;
                    case Magnetometer:
                        sensorTypes.Add(BS_SensorType.Magnetometer);
                        break;
                }
            }
        }
        return sensorTypes;
    }

    public BS_TfliteTask Task;

    [Range(5, 100)]
    public ushort SensorRate;

    [Range(0, 5000)]
    public uint CaptureDelay;

    [Range(0, 1)]
    public float Threshold;

    public void OnBeforeSerialize()
    {
        SensorRate -= (ushort)(SensorRate % 5);
        SensorRate = Math.Max(SensorRate, (ushort)5);
    }

    public void OnAfterDeserialize() { }
}