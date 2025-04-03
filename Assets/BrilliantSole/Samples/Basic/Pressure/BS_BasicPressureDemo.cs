using UnityEngine;
using static BS_Side;
using static BS_SensorType;
using static BS_SensorRate;

using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;

public class BS_BasicPressureDemo : MonoBehaviour
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

    public Button TogglePressureDataButton;
    public Slider DevicePairPressureSlider;

    private BS_DevicePair DevicePair => BS_DevicePair.Insoles;

    private void OnEnable()
    {
        setActive(true);

        TogglePressureDataButton.onClick.AddListener(TogglePressureData);
        DevicePair.OnDevicePressureData += OnDevicePressureData;

        DevicePair.OnPressureData += OnPressureData;

        updatePressureSensorMaterials();
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnDevicePressureData -= OnDevicePressureData;
        TogglePressureDataButton.onClick.RemoveListener(TogglePressureData);

        DevicePair.ClearSensorConfiguration();
        IsPressureDataEnabled = false;
        UpdateTogglePressureDataButton();

        DevicePair.OnPressureData -= OnPressureData;
    }

    private void setActive(bool active)
    {
        setActive(Left, active);
        setActive(Right, active);
    }

    private void setActive(BS_Side side, bool active)
    {
        var Insole = GetInsole(side);
        if (Insole == null) { return; }
        Insole.SetActive(active);
    }

    private void updatePressureSensorMaterials()
    {
        foreach (var device in DevicePair.Devices.Values)
        {
            var baseMeshRenderer = GetInsoleBaseImageMeshRenderer((BS_Side)device.Side);
            string baseMaterialPath;
            if (device.IsUkaton)
            {
                baseMaterialPath = "Assets/BrilliantSole/Samples/Basic/Pressure/Materials/UkatonRightInsolePressure.mat";
            }
            else
            {
                baseMaterialPath = "Assets/BrilliantSole/Samples/Basic/Pressure/Materials/RightInsolePressure.mat";
            }
            var newBaseMaterial = AssetDatabase.LoadAssetAtPath<Material>(baseMaterialPath);
            if (newBaseMaterial)
            {
                var materials = baseMeshRenderer.materials;
                materials[0] = newBaseMaterial;
                baseMeshRenderer.materials = materials;
            }
            else
            {
                Debug.LogError($"failed to load base material {baseMaterialPath}");
            }

            foreach (int index in Enumerable.Range(0, 16))
            {
                var pressureSensorGameObject = GetInsolePressureSensorGameObject((BS_Side)device.Side, index);
                var isIncluded = index < device.NumberOfPressureSensors;
                pressureSensorGameObject.SetActive(isIncluded);
                if (isIncluded)
                {
                    var meshRenderer = GetInsolePressureSensorMeshRenderer((BS_Side)device.Side, index);
                    string materialPath;
                    if (device.IsUkaton)
                    {
                        materialPath = $"Assets/BrilliantSole/Samples/Basic/Pressure/Materials/UkatonRightInsolePressure{index}.mat";

                    }
                    else
                    {
                        materialPath = $"Assets/BrilliantSole/Samples/Basic/Pressure/Materials/RightInsolePressure{index}.mat";
                    }

                    var newMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if (newMaterial)
                    {
                        var materials = meshRenderer.materials;
                        materials[0] = newMaterial;
                        meshRenderer.materials = materials;
                    }
                    else
                    {
                        //Debug.LogError($"failed to load material {materialPath}");
                    }
                }
            }
        }
    }

    private Transform GetInsoleTransform(BS_Side side, string path)
    {
        return GetInsole(side)?.transform?.Find(path);
    }
    private GameObject GetInsolePressureSensorGameObject(BS_Side side, int index)
    {
        return GetInsoleTransform(side, $"InsolePressure/Transform/Sensors/Pressure{index}").gameObject;
    }
    private MeshRenderer GetInsolePressureSensorMeshRenderer(BS_Side side, int index)
    {
        return GetInsolePressureSensorGameObject(side, index).GetComponent<MeshRenderer>();
    }

    private GameObject GetInsoleBaseImageGameObject(BS_Side side)
    {
        return GetInsoleTransform(side, $"InsolePressure/Transform/BaseImage").gameObject;
    }
    private MeshRenderer GetInsoleBaseImageMeshRenderer(BS_Side side)
    {
        return GetInsoleBaseImageGameObject(side).GetComponent<MeshRenderer>();
    }

    private void OnDevicePressureData(BS_DevicePair devicePair, BS_Side side, BS_Device device, BS_PressureData pressureData, ulong timetamp)
    {
        for (int index = 0; index < pressureData.Sensors.Length; index++)
        {
            ref var sensor = ref pressureData.Sensors[index];
            var meshRenderer = GetInsolePressureSensorMeshRenderer(side, index);
            meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, sensor.NormalizedValue));
        }
    }

    public BS_SensorRate SensorRate = _20ms;
    private bool IsPressureDataEnabled = false;
    private void TogglePressureData()
    {
        IsPressureDataEnabled = !IsPressureDataEnabled;
        DevicePair.SetSensorRate(Pressure, IsPressureDataEnabled ? SensorRate : _0ms);
        UpdateTogglePressureDataButton();
    }

    private void UpdateTogglePressureDataButton()
    {
        var togglePressureDataButtonText = TogglePressureDataButton.transform.Find("Text").GetComponentInChildren<TextMeshProUGUI>();
        togglePressureDataButtonText.text = IsPressureDataEnabled ? "disable pressure" : "enable pressure";
    }

    private void OnPressureData(BS_DevicePair devicePair, BS_DevicePairPressureData pressureData, ulong timestamp)
    {
        if (pressureData.NormalizedCenterOfPressure == null) { return; }
        DevicePairPressureSlider.value = (float)pressureData.NormalizedCenterOfPressure?.x;
    }
}
