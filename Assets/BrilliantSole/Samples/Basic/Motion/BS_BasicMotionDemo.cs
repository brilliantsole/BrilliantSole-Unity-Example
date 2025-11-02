using TMPro;
using UnityEngine;
using static BS_SensorType;
using static BS_SensorRate;
using static BS_Side;

using System.Linq;
using System.Collections.Generic;
using System;

public class BS_BasicMotionDemo : MonoBehaviour
{
    public GameObject LeftInsole;
    public GameObject RightInsole;
    private GameObject GetInsole(BS_Side side)
    {
        return side switch
        {
            Left => LeftInsole,
            Right => RightInsole,
            _ => null
        };
    }

    private BS_DevicePair DevicePair => BS_DevicePair.Insoles;

    public TMP_Dropdown positionDropdown;
    public TMP_Dropdown rotationDropdown;

    private void OnEnable()
    {
        setActive(true);

        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope += OnDeviceGyroscope;
        DevicePair.OnDeviceOrientation += OnDeviceEulerAngles;

        DevicePair.OnDeviceAcceleration += OnDevicePosition;
        DevicePair.OnDeviceLinearAcceleration += OnDevicePosition;

        positionDropdown.onValueChanged.AddListener(OnPositionDropdownValueChanged);
        rotationDropdown.onValueChanged.AddListener(OnRotationDropdownValueChanged);
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope -= OnDeviceGyroscope;
        DevicePair.OnDeviceOrientation -= OnDeviceEulerAngles;

        DevicePair.OnDeviceAcceleration -= OnDevicePosition;
        DevicePair.OnDeviceLinearAcceleration -= OnDevicePosition;

        positionDropdown.onValueChanged.RemoveListener(OnPositionDropdownValueChanged);
        rotationDropdown.onValueChanged.RemoveListener(OnRotationDropdownValueChanged);

        rotationDropdown.SetValueWithoutNotify(0);
        positionDropdown.SetValueWithoutNotify(0);
        DevicePair.ClearSensorConfiguration();
    }

    private void setActive(bool active)
    {
        if (LeftInsole != null) { LeftInsole.SetActive(active); }
        if (RightInsole != null) { RightInsole.SetActive(active); }
    }

    private Transform GetInsoleTransform(BS_Side side, string path)
    {
        return GetInsole(side)?.transform?.Find(path);
    }
    private Transform GetInsolePositionTransform(BS_Side side)
    {
        return GetInsoleTransform(side, "Rotation/Position");
    }
    private Transform GetInsoleRotationTransform(BS_Side side)
    {
        return GetInsoleTransform(side, "Rotation");
    }

    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_Side side, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(side);
        if (insoleTransform == null) { return; }

        LatestYaw[side] = quaternion.eulerAngles.y;

        var offsetYaw = OffsetYaw.GetValueOrDefault(side, 0.0f);
        var yawAdjustment = Quaternion.Euler(0, -offsetYaw, 0);

        insoleTransform.localRotation = yawAdjustment * quaternion;
    }
    private void OnDeviceEulerAngles(BS_DevicePair devicePair, BS_Side side, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(side);
        if (insoleTransform == null) { return; }

        LatestYaw[side] = eulerAngles.y;

        var offsetYaw = OffsetYaw.GetValueOrDefault(side, 0.0f);
        var yawAdjustment = Quaternion.Euler(0, -offsetYaw, 0);

        insoleTransform.localRotation = yawAdjustment * Quaternion.Euler(eulerAngles);
    }
    private void OnDeviceGyroscope(BS_DevicePair devicePair, BS_Side side, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        var insoleTransform = GetInsoleRotationTransform(side);
        if (insoleTransform == null) { return; }
        insoleTransform.localRotation = Quaternion.Euler(eulerAngles * 0.2f);
    }
    private void OnDevicePosition(BS_DevicePair devicePair, BS_Side side, BS_Device device, Vector3 position, ulong timestamp)
    {
        var insoleTransform = GetInsolePositionTransform(side);
        if (insoleTransform == null) { return; }
        insoleTransform.localPosition = Vector3.Lerp(insoleTransform.localPosition, position * 100.0f, 0.4f);
    }

    private readonly Dictionary<BS_Side, float> LatestYaw = new();
    private readonly Dictionary<BS_Side, float> OffsetYaw = new();
    public void Calibrate()
    {
        Debug.Log("Calibrating...");
        foreach (BS_Side side in Enum.GetValues(typeof(BS_Side)))
        {
            OffsetYaw[side] = LatestYaw.GetValueOrDefault(side, 0.0f);
        }
    }

    private readonly BS_SensorConfiguration rotationSensorConfiguration = new() {
        {GameRotation, _0ms},
        {Rotation, _0ms},
        {Orientation, _0ms},
        {BS_SensorType.Gyroscope, _0ms},
    };
    private void OnRotationDropdownValueChanged(int selectedIndex)
    {
        string selectedRotation = rotationDropdown.options[selectedIndex].text;
        Debug.Log($"selectedRotation: {selectedRotation}");

        foreach (var key in rotationSensorConfiguration.Keys.ToList()) { rotationSensorConfiguration[key] = _0ms; }
        BS_SensorType? sensorType = selectedRotation switch
        {
            "Game Rotation" => GameRotation,
            "Rotation" => Rotation,
            "Orientation" => Orientation,
            "Gyroscope" => BS_SensorType.Gyroscope,
            _ => null
        };
        if (sensorType != null)
        {
            Debug.Log($"sensorType: {sensorType}");
            rotationSensorConfiguration[(BS_SensorType)sensorType] = SensorRate;
        }
        else
        {
            Debug.Log("clearing rotation");
        }
        DevicePair.SetSensorConfiguration(rotationSensorConfiguration);
    }

    public BS_SensorRate SensorRate = _20ms;
    private readonly BS_SensorConfiguration positionSensorConfiguration = new() {
        {LinearAcceleration, _0ms},
        {Acceleration, _0ms},
    };
    private void OnPositionDropdownValueChanged(int selectedIndex)
    {
        string selectedPosition = positionDropdown.options[selectedIndex].text;
        Debug.Log($"selectedPosition: {selectedPosition}");

        foreach (var key in positionSensorConfiguration.Keys.ToList()) { positionSensorConfiguration[key] = _0ms; }
        BS_SensorType? sensorType = selectedPosition switch
        {
            "Linear Acceleration" => LinearAcceleration,
            "Acceleration" => Acceleration,
            _ => null
        };
        Debug.Log($"sensorType: {sensorType}");

        if (sensorType != null)
        {
            Debug.Log($"setting sensorType: {sensorType} to {SensorRate}");
            positionSensorConfiguration[(BS_SensorType)sensorType] = SensorRate;
        }
        else
        {
            Debug.Log("clearing position");
        }
        DevicePair.SetSensorConfiguration(positionSensorConfiguration);
    }
}
