using UnityEngine;
using static BS_InsoleSide;
using static BS_SensorType;
using static BS_SensorRate;

using BS_SensorConfiguration = System.Collections.Generic.Dictionary<BS_SensorType, BS_SensorRate>;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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

        TogglePressureDataButton.onClick.AddListener(TogglePressureData);
        DevicePair.OnDevicePressureData += OnDevicePressureData;
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnDevicePressureData -= OnDevicePressureData;
        TogglePressureDataButton.onClick.RemoveListener(TogglePressureData);

        DevicePair.ClearSensorConfiguration();
        IsPressureDataEnabled = false;
        UpdateTogglePressureDataButton();
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
    private MeshRenderer GetInsolePressureSensorMeshRenderer(BS_InsoleSide insoleSide, int index)
    {
        return GetInsoleTransform(insoleSide, $"InsolePressure/Transform/Sensors/Pressure{index}").gameObject.GetComponent<MeshRenderer>();
    }

    private void OnDevicePressureData(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, BS_PressureData pressureData, ulong timetamp)
    {
        for (int index = 0; index < pressureData.Sensors.Length; index++)
        {
            ref var sensor = ref pressureData.Sensors[index];
            var meshRenderer = GetInsolePressureSensorMeshRenderer(insoleSide, index);
            meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, sensor.NormalizedValue));
        }
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