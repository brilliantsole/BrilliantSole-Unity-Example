using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BS_Device
{
    private readonly BS_SensorDataManager SensorDataManager = new();

    public event Action<BS_Device, BS_PressureData, ulong> OnPressureData;

    public event Action<BS_Device, Vector3, ulong> OnAcceleration;
    public event Action<BS_Device, Vector3, ulong> OnGravity;
    public event Action<BS_Device, Vector3, ulong> OnLinearAcceleration;
    public event Action<BS_Device, Vector3, ulong> OnGyroscope;
    public event Action<BS_Device, Vector3, ulong> OnMagnetometer;

    public event Action<BS_Device, Quaternion, ulong> OnGameRotation;
    public event Action<BS_Device, Quaternion, ulong> OnRotation;

    public event Action<BS_Device, Vector3, ulong> OnOrientation;

    public event Action<BS_Device, uint, ulong> OnStepCount;
    public event Action<BS_Device, ulong> OnStepDetection;
    public event Action<BS_Device, HashSet<BS_Activity>, ulong> OnActivity;

    public event Action<BS_Device, BS_DeviceOrientation, ulong> OnDeviceOrienation;


    public event Action<BS_Device, float, ulong> OnBarometerData;

    private void SetupSensorDataManager()
    {
        Managers.Add(SensorDataManager);

        SensorDataManager.PressureSensorDataManager.OnPressureData = (BS_PressureData pressureData, ulong timestamp) => OnPressureData(this, pressureData, timestamp);

        SensorDataManager.MotionSensorDataManager.OnAcceleration = (Vector3 vector, ulong timestamp) => OnAcceleration(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnGravity = (Vector3 vector, ulong timestamp) => OnGravity(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnLinearAcceleration = (Vector3 vector, ulong timestamp) => OnLinearAcceleration(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnGyroscope = (Vector3 vector, ulong timestamp) => OnGyroscope(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnMagnetometer = (Vector3 vector, ulong timestamp) => OnMagnetometer(this, vector, timestamp);

        SensorDataManager.MotionSensorDataManager.OnGameRotation = (Quaternion quaternion, ulong timestamp) => OnGameRotation(this, quaternion, timestamp);
        SensorDataManager.MotionSensorDataManager.OnRotation = (Quaternion quaternion, ulong timestamp) => OnRotation(this, quaternion, timestamp);

        SensorDataManager.MotionSensorDataManager.OnOrientation = (Vector3 eulerAngles, ulong timestamp) => OnOrientation(this, eulerAngles, timestamp);

        SensorDataManager.MotionSensorDataManager.OnActivity = (HashSet<BS_Activity> activity, ulong timestamp) => OnActivity(this, activity, timestamp);

        SensorDataManager.MotionSensorDataManager.OnStepCount = (uint stepCount, ulong timestamp) => OnStepCount(this, stepCount, timestamp);

        SensorDataManager.MotionSensorDataManager.OnStepDetection = (ulong timestamp) => OnStepDetection(this, timestamp);

        SensorDataManager.MotionSensorDataManager.OnDeviceOrienation = (BS_DeviceOrientation deviceOrientation, ulong timestamp) => OnDeviceOrienation(this, deviceOrientation, timestamp);

        SensorDataManager.BarometerSensorDataManager.OnBarometer = (float barometer, ulong timestamp) => OnBarometerData(this, barometer, timestamp);
    }
}
