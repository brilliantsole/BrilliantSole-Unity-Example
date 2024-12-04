using TMPro;
using UnityEngine;
using static BS_SensorType;
using static BS_SensorRate;
using static BS_InsoleSide;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;
using System.Linq;

public class BS_BasicMotionDemo : MonoBehaviour
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

    public TMP_Dropdown positionDropdown;
    public TMP_Dropdown rotationDropdown;

    private void OnEnable()
    {
        setActive(true);

        //SetDropdwnActive(DevicePair.IsFullyConnected);
        DevicePair.OnIsFullyConnected += OnIsFullyConnected;

        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope += OnDeviceEulerAngles;
        DevicePair.OnDeviceOrientation += OnDeviceEulerAngles;

        DevicePair.OnDeviceLinearAcceleration += OnDevicePosition;

        positionDropdown.onValueChanged.AddListener(OnPositionDropdownValueChanged);
        rotationDropdown.onValueChanged.AddListener(OnRotationDropdownValueChanged);
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnIsFullyConnected -= OnIsFullyConnected;

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope -= OnDeviceEulerAngles;
        DevicePair.OnDeviceOrientation -= OnDeviceEulerAngles;

        DevicePair.OnDeviceLinearAcceleration -= OnDevicePosition;

        positionDropdown.onValueChanged.RemoveListener(OnPositionDropdownValueChanged);
        rotationDropdown.onValueChanged.RemoveListener(OnRotationDropdownValueChanged);

        rotationDropdown.SetValueWithoutNotify(0);
        positionDropdown.SetValueWithoutNotify(0);
        DevicePair.ClearSensorConfiguration();
    }

    private void OnIsFullyConnected(BS_DevicePair devicePair, bool isFullyConnected)
    {
        //SetDropdwnActive(isFullyConnected);
    }
    private void SetDropdwnActive(bool active)
    {
        rotationDropdown.interactable = active;
        positionDropdown.interactable = active;
    }

    private void setActive(bool active)
    {
        if (LeftInsole != null) { LeftInsole.SetActive(active); }
        if (RightInsole != null) { RightInsole.SetActive(active); }
    }

    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        var insoleTransform = GetInsole(insoleSide)?.transform;
        if (insoleTransform == null)
        {
            return;
        }
        // FILL - calibration stuff
        insoleTransform.rotation = quaternion;
    }
    private void OnDeviceEulerAngles(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        // FILL
        var insoleTransform = GetInsole(insoleSide)?.transform;
        if (insoleTransform == null)
        {
            return;
        }
        // FILL - calibration stuff
        insoleTransform.rotation = Quaternion.Euler(eulerAngles);
    }
    private void OnDevicePosition(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 position, ulong timestamp)
    {
        // FILL
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
            rotationSensorConfiguration[(BS_SensorType)sensorType] = _20ms;
        }
        else
        {
            Debug.Log("clearing rotation");
        }
        DevicePair.SetSensorConfiguration(rotationSensorConfiguration);
    }

    private readonly BS_SensorConfiguration positionSensorConfiguration = new() {
        {LinearAcceleration, _0ms},
    };
    private void OnPositionDropdownValueChanged(int selectedIndex)
    {
        string selectedPosition = positionDropdown.options[selectedIndex].text;
        Debug.Log($"selectedPosition: {selectedPosition}");

        foreach (var key in positionSensorConfiguration.Keys.ToList()) { positionSensorConfiguration[key] = _0ms; }
        BS_SensorType? sensorType = selectedPosition switch
        {
            "Linear Acceleration" => LinearAcceleration,
            _ => null
        };
        Debug.Log($"sensorType: {sensorType}");

        if (sensorType != null)
        {
            Debug.Log($"sensorType: {sensorType}");
            positionSensorConfiguration[(BS_SensorType)sensorType] = _20ms;
        }
        else
        {
            Debug.Log("clearing position");
        }
        DevicePair.SetSensorConfiguration(positionSensorConfiguration);
    }
}
