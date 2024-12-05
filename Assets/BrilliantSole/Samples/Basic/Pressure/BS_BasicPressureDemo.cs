using UnityEngine;
using static BS_InsoleSide;
using static BS_SensorType;
using static BS_SensorRate;
using System.Collections.Generic;
using System;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;

public class BS_BasicPressureDemo : MonoBehaviour
{
    public GameObject LeftInsole;
    public GameObject RightInsole;
    private GameObject GetInsole(BS_InsoleSide insoleSide)
    {
        return insoleSide switch
        {
            Left => LeftInsole,
            Right => RightInsole,
            _ => null
        };
    }

    private BS_DevicePair DevicePair => BS_DevicePair.Instance;

    private void OnEnable()
    {
        setActive(true);

        //SetDropdwnActive(DevicePair.IsFullyConnected);
        DevicePair.OnIsFullyConnected += OnIsFullyConnected;

        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope += OnDeviceGyroscope;
        DevicePair.OnDeviceOrientation += OnDeviceEulerAngles;

        DevicePair.OnDeviceAcceleration += OnDevicePosition;
        DevicePair.OnDeviceLinearAcceleration += OnDevicePosition;
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnIsFullyConnected -= OnIsFullyConnected;

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope -= OnDeviceGyroscope;
        DevicePair.OnDeviceOrientation -= OnDeviceEulerAngles;

        DevicePair.OnDeviceAcceleration -= OnDevicePosition;
        DevicePair.OnDeviceLinearAcceleration -= OnDevicePosition;

        DevicePair.ClearSensorConfiguration();
    }

    private void OnIsFullyConnected(BS_DevicePair devicePair, bool isFullyConnected)
    {
        // FILL
    }

    private void setActive(bool active)
    {
        if (LeftInsole != null) { LeftInsole.SetActive(active); }
        if (RightInsole != null) { RightInsole.SetActive(active); }
    }

    private Transform GetInsoleTransform(BS_InsoleSide insoleSide, string path)
    {
        return GetInsole(insoleSide)?.transform?.Find(path);
    }
    private Transform GetInsolePositionTransform(BS_InsoleSide insoleSide)
    {
        return GetInsoleTransform(insoleSide, "Rotation/Position");
    }
    private Transform GetInsoleRotationTransform(BS_InsoleSide insoleSide)
    {
        return GetInsoleTransform(insoleSide, "Rotation");
    }

    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(insoleSide);
        if (insoleTransform == null) { return; }

        LatestYaw[insoleSide] = quaternion.eulerAngles.y;
        insoleTransform.localRotation = quaternion;

        var offsetYaw = OffsetYaw.GetValueOrDefault(insoleSide, 0.0f);
        insoleTransform.Rotate(0, -offsetYaw, 0);
    }
    private void OnDeviceEulerAngles(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(insoleSide);
        if (insoleTransform == null) { return; }

        LatestYaw[insoleSide] = eulerAngles.y;
        insoleTransform.localRotation = Quaternion.Euler(eulerAngles);

        var offsetYaw = OffsetYaw.GetValueOrDefault(insoleSide, 0.0f);
        insoleTransform.Rotate(0, -offsetYaw, 0);
    }
    private void OnDeviceGyroscope(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(insoleSide);
        if (insoleTransform == null) { return; }
        insoleTransform.localRotation = Quaternion.Euler(eulerAngles * 0.2f);
    }
    private void OnDevicePosition(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 position, ulong timestamp)
    {
        var insoleTransform = GetInsolePositionTransform(insoleSide);
        if (insoleTransform == null) { return; }
        insoleTransform.localPosition = Vector3.Lerp(insoleTransform.localPosition, position * 100.0f, 0.4f);
    }

    private readonly Dictionary<BS_InsoleSide, float> LatestYaw = new();
    private readonly Dictionary<BS_InsoleSide, float> OffsetYaw = new();
    public void Calibrate()
    {
        Debug.Log("Calibrating...");
        foreach (BS_InsoleSide insoleSide in Enum.GetValues(typeof(BS_InsoleSide)))
        {
            OffsetYaw[insoleSide] = LatestYaw.GetValueOrDefault(insoleSide, 0.0f);
        }
    }

    public void TogglePressureSensorData()
    {
        // FILL
    }
}
