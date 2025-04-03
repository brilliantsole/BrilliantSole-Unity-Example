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

        SensorDataManager.PressureSensorDataManager.OnPressureData = (pressureData, timestamp) => OnPressureData?.Invoke(this, pressureData, timestamp);

        SensorDataManager.MotionSensorDataManager.OnAcceleration = (vector, timestamp) => OnAcceleration?.Invoke(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnGravity = (vector, timestamp) => OnGravity?.Invoke(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnLinearAcceleration = (vector, timestamp) => OnLinearAcceleration?.Invoke(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnGyroscope = (vector, timestamp) => OnGyroscope?.Invoke(this, vector, timestamp);
        SensorDataManager.MotionSensorDataManager.OnMagnetometer = (vector, timestamp) => OnMagnetometer?.Invoke(this, vector, timestamp);

        SensorDataManager.MotionSensorDataManager.OnGameRotation = (quaternion, timestamp) => OnGameRotation?.Invoke(this, quaternion, timestamp);
        SensorDataManager.MotionSensorDataManager.OnRotation = (quaternion, timestamp) => OnRotation?.Invoke(this, quaternion, timestamp);

        SensorDataManager.MotionSensorDataManager.OnOrientation = (eulerAngles, timestamp) => OnOrientation?.Invoke(this, eulerAngles, timestamp);

        SensorDataManager.MotionSensorDataManager.OnActivity = (activity, timestamp) => OnActivity?.Invoke(this, activity, timestamp);

        SensorDataManager.MotionSensorDataManager.OnStepCount = (stepCount, timestamp) => OnStepCount?.Invoke(this, stepCount, timestamp);

        SensorDataManager.MotionSensorDataManager.OnStepDetection = (timestamp) => OnStepDetection?.Invoke(this, timestamp);

        SensorDataManager.MotionSensorDataManager.OnDeviceOrienation = (deviceOrientation, timestamp) => OnDeviceOrienation?.Invoke(this, deviceOrientation, timestamp);

        SensorDataManager.BarometerSensorDataManager.OnBarometer = (barometer, timestamp) => OnBarometerData?.Invoke(this, barometer, timestamp);
    }

    public int NumberOfPressureSensors => SensorDataManager.PressureSensorDataManager.NumberOfPressureSensors;
}
