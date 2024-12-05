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

        device.OnDeviceOrienation += onDeviceDeviceOrientation;

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

        device.OnDeviceOrienation -= onDeviceDeviceOrientation;

        device.OnBarometerData -= onDeviceBarometerData;
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, BS_PressureData, ulong> OnDevicePressureData;
    private void onDevicePressureData(BS_Device device, BS_PressureData pressureData, ulong timestamp)
    {
        var insoleSide = (BS_InsoleSide)device.InsoleSide;
        OnDevicePressureData?.Invoke(this, insoleSide, device, pressureData, timestamp);
        SensorDataManager.PressureSensorDataManager.OnDevicePressureData(insoleSide, pressureData, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceAcceleration;
    private void onDeviceAcceleration(BS_Device device, Vector3 acceleration, ulong timestamp)
    {
        OnDeviceAcceleration?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, acceleration, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceGravity;
    private void onDeviceGravity(BS_Device device, Vector3 gravity, ulong timestamp)
    {
        OnDeviceGravity?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, gravity, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceLinearAcceleration;
    private void onDeviceLinearAcceleration(BS_Device device, Vector3 linearAcceleration, ulong timestamp)
    {
        OnDeviceLinearAcceleration?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, linearAcceleration, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceGyroscope;
    private void onDeviceGyroscope(BS_Device device, Vector3 gyroscope, ulong timestamp)
    {
        OnDeviceGyroscope?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, gyroscope, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceMagnetometer;
    private void onDeviceMagnetometer(BS_Device device, Vector3 magnetometer, ulong timestamp)
    {
        OnDeviceMagnetometer?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, magnetometer, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Quaternion, ulong> OnDeviceGameRotation;
    private void onDeviceGameRotation(BS_Device device, Quaternion gameRotation, ulong timestamp)
    {
        OnDeviceGameRotation?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, gameRotation, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Quaternion, ulong> OnDeviceRotation;
    private void onDeviceRotation(BS_Device device, Quaternion rotation, ulong timestamp)
    {
        OnDeviceRotation?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, rotation, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, Vector3, ulong> OnDeviceOrientation;
    private void onDeviceOrientation(BS_Device device, Vector3 deviceOrientation, ulong timestamp)
    {
        OnDeviceOrientation?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, deviceOrientation, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, ulong> OnDeviceStepDetection;
    private void onDeviceStepDetection(BS_Device device, ulong timestamp)
    {
        OnDeviceStepDetection?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, uint, ulong> OnDeviceStepCount;
    private void onDeviceStepCount(BS_Device device, uint stepCount, ulong timestamp)
    {
        OnDeviceStepCount?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, stepCount, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, HashSet<BS_Activity>, ulong> OnDeviceActivity;
    private void onDeviceActivity(BS_Device device, HashSet<BS_Activity> activity, ulong timestamp)
    {
        OnDeviceActivity?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, activity, timestamp);
    }
    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, BS_DeviceOrientation, ulong> OnDeviceDeviceOrientation;
    private void onDeviceDeviceOrientation(BS_Device device, BS_DeviceOrientation deviceOrientation, ulong timestamp)
    {
        OnDeviceDeviceOrientation?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, deviceOrientation, timestamp);
    }

    public event Action<BS_DevicePair, BS_InsoleSide, BS_Device, float, ulong> OnDeviceBarometerData;
    private void onDeviceBarometerData(BS_Device device, float barometerData, ulong timestamp)
    {
        OnDeviceBarometerData?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, barometerData, timestamp);
    }
}
