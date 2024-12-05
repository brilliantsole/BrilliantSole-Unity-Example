using UnityEngine;
using static BS_InsoleSide;
using static BS_SensorType;
using static BS_SensorRate;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;
using UnityEngine.UI;
using TMPro;

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

    public Button TogglePressureDataButton;

    private BS_DevicePair DevicePair => BS_DevicePair.Instance;

    private void OnEnable()
    {
        setActive(true);

        DevicePair.OnIsFullyConnected += OnIsFullyConnected;

        TogglePressureDataButton.onClick.AddListener(TogglePressureData);
        DevicePair.OnDevicePressureData += OnDevicePressureData;
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnIsFullyConnected -= OnIsFullyConnected;

        DevicePair.OnDevicePressureData -= OnDevicePressureData;
        TogglePressureDataButton.onClick.RemoveListener(TogglePressureData);

        DevicePair.ClearSensorConfiguration();
        IsPressureDataEnabled = false;
        UpdateTogglePressureDataButton();
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

    private void OnDevicePressureData(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, BS_PressureData pressureData, ulong timetamp)
    {
        // FILL
    }

    private bool IsPressureDataEnabled = false;
    private void TogglePressureData()
    {
        IsPressureDataEnabled = !IsPressureDataEnabled;
        DevicePair.SetSensorRate(Pressure, IsPressureDataEnabled ? _20ms : _0ms);
        UpdateTogglePressureDataButton();
    }

    private void UpdateTogglePressureDataButton()
    {
        var togglePressureDataButtonText = TogglePressureDataButton.transform.Find("Text").GetComponentInChildren<TextMeshProUGUI>();
        togglePressureDataButtonText.text = IsPressureDataEnabled ? "disable pressure" : "enable pressure";
    }
}
