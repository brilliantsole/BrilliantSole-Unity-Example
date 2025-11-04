using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BS_DevicePair
{
    private void AddDeviceSensorDataListeners(BS_Device device)
    {
        device.OnPressureData += onDevicePressureData;

        device.OnAcceleration += onDeviceAcceleration;
        device.OnGravity += onDeviceGravity;
        device.OnLinearAcceleration += onDeviceLinearAcceleration;
        device.OnGyroscope += onDeviceGyroscope;
        device.OnMagnetometer += onDeviceMagnetometer;

        device.OnGameRotation += onDeviceGameRotation;
        device.OnRotation += onDeviceRotation;

        device.OnOrientation += onDeviceOrientation;

        device.OnActivity += onDeviceActivity;

        device.OnStepCount += onDeviceStepCount;
        device.OnStepDetection += onDeviceStepDetection;

        device.OnDeviceOrientation += onDeviceDeviceOrientation;

        device.OnBarometerData += onDeviceBarometerData;
    }
    private void RemoveDeviceSensorDataListeners(BS_Device device)
    {
        device.OnPressureData -= onDevicePressureData;

        device.OnAcceleration -= onDeviceAcceleration;
        device.OnGravity -= onDeviceGravity;
        device.OnLinearAcceleration -= onDeviceLinearAcceleration;
        device.OnGyroscope -= onDeviceGyroscope;
        device.OnMagnetometer -= onDeviceMagnetometer;

        device.OnGameRotation -= onDeviceGameRotation;
        device.OnRotation -= onDeviceRotation;

        device.OnOrientation -= onDeviceOrientation;

        device.OnActivity -= onDeviceActivity;

        device.OnStepCount -= onDeviceStepCount;
        device.OnStepDetection -= onDeviceStepDetection;

        device.OnDeviceOrientation -= onDeviceDeviceOrientation;

        device.OnBarometerData -= onDeviceBarometerData;
    }

    public delegate void OnDevicePressureDataDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_PressureData pressureData,
        ulong timestamp
    );
    public delegate void OnDeviceAccelerationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 acceleration,
        ulong timestamp
    );
    public delegate void OnDeviceGravityDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 gravity,
        ulong timestamp
    );
    public delegate void OnDeviceLinearAccelerationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 linearAcceleration,
        ulong timestamp
    );
    public delegate void OnDeviceGyroscopeDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 gyroscope,
        ulong timestamp
    );
    public delegate void OnDeviceMagnetometerDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 magnetometer,
        ulong timestamp
    );
    public delegate void OnDeviceGameRotationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Quaternion gameRotation,
        ulong timestamp
    );
    public delegate void OnDeviceRotationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Quaternion rotation,
        ulong timestamp
    );
    public delegate void OnDeviceOrientationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        Vector3 orientation,
        ulong timestamp
    );
    public delegate void OnDeviceStepCountDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        uint stepCount,
        ulong timestamp
    );
    public delegate void OnDeviceStepDetectionDelegate(
    BS_DevicePair devicePair,
    BS_Side side,
    BS_Device device,
    ulong timestamp
    );
    public delegate void OnDeviceActivityDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        HashSet<BS_Activity> activities,
        ulong timestamp
    );
    public delegate void OnDeviceDeviceOrientationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_DeviceOrientation deviceOrientation,
        ulong timestamp
    );
    public delegate void OnDeviceBarometerDataDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        float barometer,
        ulong timestamp
    );

    public event OnDevicePressureDataDelegate OnDevicePressureData;
    public event OnDeviceAccelerationDelegate OnDeviceAcceleration;
    public event OnDeviceGravityDelegate OnDeviceGravity;
    public event OnDeviceLinearAccelerationDelegate OnDeviceLinearAcceleration;
    public event OnDeviceGyroscopeDelegate OnDeviceGyroscope;
    public event OnDeviceMagnetometerDelegate OnDeviceMagnetometer;
    public event OnDeviceGameRotationDelegate OnDeviceGameRotation;
    public event OnDeviceRotationDelegate OnDeviceRotation;
    public event OnDeviceOrientationDelegate OnDeviceOrientation;
    public event OnDeviceStepDetectionDelegate OnDeviceStepDetection;
    public event OnDeviceStepCountDelegate OnDeviceStepCount;
    public event OnDeviceActivityDelegate OnDeviceActivity;
    public event OnDeviceDeviceOrientationDelegate OnDeviceDeviceOrientation;
    public event OnDeviceBarometerDataDelegate OnDeviceBarometerData;

    private void onDevicePressureData(BS_Device device, BS_PressureData pressureData, ulong timestamp)
    {
        var side = (BS_Side)device.Side;
        OnDevicePressureData?.Invoke(this, side, device, pressureData, timestamp);
        SensorDataManager.PressureSensorDataManager.OnDevicePressureData(side, pressureData, timestamp);
    }
    private void onDeviceAcceleration(BS_Device device, Vector3 acceleration, ulong timestamp)
    {
        OnDeviceAcceleration?.Invoke(this, (BS_Side)device.Side, device, acceleration, timestamp);
    }
    private void onDeviceGravity(BS_Device device, Vector3 gravity, ulong timestamp)
    {
        OnDeviceGravity?.Invoke(this, (BS_Side)device.Side, device, gravity, timestamp);
    }
    private void onDeviceLinearAcceleration(BS_Device device, Vector3 linearAcceleration, ulong timestamp)
    {
        OnDeviceLinearAcceleration?.Invoke(this, (BS_Side)device.Side, device, linearAcceleration, timestamp);
    }
    private void onDeviceGyroscope(BS_Device device, Vector3 gyroscope, ulong timestamp)
    {
        OnDeviceGyroscope?.Invoke(this, (BS_Side)device.Side, device, gyroscope, timestamp);
    }
    private void onDeviceMagnetometer(BS_Device device, Vector3 magnetometer, ulong timestamp)
    {
        OnDeviceMagnetometer?.Invoke(this, (BS_Side)device.Side, device, magnetometer, timestamp);
    }

    private void onDeviceGameRotation(BS_Device device, Quaternion gameRotation, ulong timestamp)
    {
        OnDeviceGameRotation?.Invoke(this, (BS_Side)device.Side, device, gameRotation, timestamp);
    }
    private void onDeviceRotation(BS_Device device, Quaternion rotation, ulong timestamp)
    {
        OnDeviceRotation?.Invoke(this, (BS_Side)device.Side, device, rotation, timestamp);
    }
    private void onDeviceOrientation(BS_Device device, Vector3 deviceOrientation, ulong timestamp)
    {
        OnDeviceOrientation?.Invoke(this, (BS_Side)device.Side, device, deviceOrientation, timestamp);
    }
    private void onDeviceStepDetection(BS_Device device, ulong timestamp)
    {
        OnDeviceStepDetection?.Invoke(this, (BS_Side)device.Side, device, timestamp);
    }
    private void onDeviceStepCount(BS_Device device, uint stepCount, ulong timestamp)
    {
        OnDeviceStepCount?.Invoke(this, (BS_Side)device.Side, device, stepCount, timestamp);
    }
    private void onDeviceActivity(BS_Device device, HashSet<BS_Activity> activity, ulong timestamp)
    {
        OnDeviceActivity?.Invoke(this, (BS_Side)device.Side, device, activity, timestamp);
    }
    private void onDeviceDeviceOrientation(BS_Device device, BS_DeviceOrientation deviceOrientation, ulong timestamp)
    {
        OnDeviceDeviceOrientation?.Invoke(this, (BS_Side)device.Side, device, deviceOrientation, timestamp);
    }
    private void onDeviceBarometerData(BS_Device device, float barometerData, ulong timestamp)
    {
        OnDeviceBarometerData?.Invoke(this, (BS_Side)device.Side, device, barometerData, timestamp);
    }
}
