using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BS_Device
{
    private readonly BS_SensorDataManager SensorDataManager = new();

    public delegate void OnPressureDataDelegate(BS_Device device, BS_PressureData pressureData, ulong timestamp);
    public delegate void OnAccelerationDelegate(BS_Device device, Vector3 acceleration, ulong timestamp);
    public delegate void OnGravityDelegate(BS_Device device, Vector3 gravity, ulong timestamp);
    public delegate void OnLinearAccelerationDelegate(BS_Device device, Vector3 linearAcceleration, ulong timestamp);
    public delegate void OnGyroscopeDelegate(BS_Device device, Vector3 gyroscope, ulong timestamp);
    public delegate void OnMagnetometerDelegate(BS_Device device, Vector3 magnetometer, ulong timestamp);
    public delegate void OnGameRotationDelegate(BS_Device device, Quaternion rotation, ulong timestamp);
    public delegate void OnRotationDelegate(BS_Device device, Quaternion rotation, ulong timestamp);
    public delegate void OnOrientationDelegate(BS_Device device, Vector3 orientation, ulong timestamp);
    public delegate void OnStepCountDelegate(BS_Device device, uint stepCount, ulong timestamp);
    public delegate void OnStepDetectionDelegate(BS_Device device, ulong timestamp);
    public delegate void OnActivityDelegate(BS_Device device, HashSet<BS_Activity> activities, ulong timestamp);
    public delegate void OnDeviceOrientationDelegate(BS_Device device, BS_DeviceOrientation deviceOrientation, ulong timestamp);
    public delegate void OnBarometerDataDelegate(BS_Device device, float barometer, ulong timestamp);

    public event OnPressureDataDelegate OnPressureData;
    public event OnAccelerationDelegate OnAcceleration;
    public event OnGravityDelegate OnGravity;
    public event OnLinearAccelerationDelegate OnLinearAcceleration;
    public event OnGyroscopeDelegate OnGyroscope;
    public event OnMagnetometerDelegate OnMagnetometer;
    public event OnGameRotationDelegate OnGameRotation;
    public event OnRotationDelegate OnRotation;
    public event OnOrientationDelegate OnOrientation;
    public event OnStepCountDelegate OnStepCount;
    public event OnStepDetectionDelegate OnStepDetection;
    public event OnActivityDelegate OnActivity;
    public event OnDeviceOrientationDelegate OnDeviceOrientation;
    public event OnBarometerDataDelegate OnBarometerData;


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

        SensorDataManager.MotionSensorDataManager.OnDeviceOrientation = (deviceOrientation, timestamp) => OnDeviceOrientation?.Invoke(this, deviceOrientation, timestamp);

        SensorDataManager.BarometerSensorDataManager.OnBarometer = (barometer, timestamp) => OnBarometerData?.Invoke(this, barometer, timestamp);
    }

    public int NumberOfPressureSensors => SensorDataManager.PressureSensorDataManager.NumberOfPressureSensors;
}
