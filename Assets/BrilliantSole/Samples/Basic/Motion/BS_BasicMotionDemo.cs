using TMPro;
using UnityEngine;
using static BS_SensorType;

public class BS_BasicMotionDemo : MonoBehaviour
{
    public GameObject LeftInsole;
    public GameObject RightInsole;

    private BS_DevicePair DevicePair => BS_DevicePair.Instance;

    public TMP_Dropdown positionDropdown;
    public TMP_Dropdown rotationDropdown;

    private void OnEnable()
    {
        setActive(true);

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

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope -= OnDeviceEulerAngles;
        DevicePair.OnDeviceOrientation -= OnDeviceEulerAngles;

        DevicePair.OnDeviceLinearAcceleration -= OnDevicePosition;

        positionDropdown.onValueChanged.RemoveListener(OnPositionDropdownValueChanged);
        rotationDropdown.onValueChanged.RemoveListener(OnRotationDropdownValueChanged);

    }

    private void setActive(bool active)
    {
        LeftInsole?.SetActive(active);
        RightInsole?.SetActive(active);
    }

    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        // FILL
    }
    private void OnDeviceEulerAngles(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        // FILL
    }
    private void OnDevicePosition(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 position, ulong timestamp)
    {
        // FILL
    }

    private void OnRotationDropdownValueChanged(int selectedIndex)
    {
        string selectedRotation = rotationDropdown.options[selectedIndex].text;
        Debug.Log($"selectedRotation: {selectedRotation}");
        BS_SensorType? sensorType = selectedRotation switch
        {
            "Game Rotation" => GameRotation,
            "Rotation" => Rotation,
            "Orientation" => Orientation,
            "Gyroscope" => BS_SensorType.Gyroscope,
            _ => null
        };
        Debug.Log($"sensorType: {sensorType}");
        // FILL
    }

    private void OnPositionDropdownValueChanged(int selectedIndex)
    {
        string selectedPosition = positionDropdown.options[selectedIndex].text;
        Debug.Log($"selectedPosition: {selectedPosition}");
        BS_SensorType? sensorType = selectedPosition switch
        {
            "Linear Acceleration" => LinearAcceleration,
            _ => null
        };
        Debug.Log($"sensorType: {sensorType}");
        // FILL
    }
}
