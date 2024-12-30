using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BS_HueBridgeScrollView : MonoBehaviour
{
    private HueBridge hueBridge;
    public GameObject HueBridge;
    public GameObject ItemPrefab;
    public Transform Content;
    private readonly Dictionary<string, GameObject> instantiatedItems = new();

    private IReadOnlyList<HueLamp> currentHueLamps;

    private readonly BS_DevicePair DevicePair = BS_DevicePair.Instance;

    void Start()
    {
        hueBridge = HueBridge.GetComponent<HueBridge>();
        OnDiscoveredHueLamps(hueBridge, hueBridge.HueLamps);
        hueBridge.OnDiscoveredHueLamps += OnDiscoveredHueLamps;
    }

    private void OnEnable()
    {
        DevicePair.SetTfliteInferencingEnabled(true);
    }
    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        DevicePair.SetTfliteInferencingEnabled(false);
    }

    public void DiscoverLights()
    {
        hueBridge.DiscoverLights();
    }

    public void SetHueBridgeHostname(string hostName)
    {
        Debug.Log($"setting hostName to {hostName}");
        hueBridge.hostName = hostName;
    }
    public void SetHueBridgeUsername(string username)
    {
        Debug.Log($"setting username to {username}");
        hueBridge.username = username;
    }

    private void OnDiscoveredHueLamps(HueBridge hueBridge, IReadOnlyList<HueLamp> hueLamps)
    {
        List<HueLamp> hueLampsToRemove = currentHueLamps == null ? new() : new(currentHueLamps);
        foreach (var hueLamp in hueLamps)
        {
            OnDiscoveredHueLamp(hueBridge, hueLamp);
            hueLampsToRemove.Remove(hueLamp);
        }
        foreach (var hueLamp in hueLampsToRemove.ToList())
        {
            if (instantiatedItems.TryGetValue(hueLamp.devicePath, out var item))
            {
                var toggleButton = GetToggleButton(item);
                toggleButton.onClick.RemoveAllListeners();

                var brightnessSlider = GetBrightnessSlider(item);
                brightnessSlider.onValueChanged.RemoveAllListeners();

                var hueSlider = GetHueSlider(item);
                hueSlider.onValueChanged.RemoveAllListeners();

                var saturationSlider = GetSaturationSlider(item);
                saturationSlider.onValueChanged.RemoveAllListeners();

                hueLamp.OnColorUpdate = null;

                Destroy(item);
                instantiatedItems.Remove(hueLamp.devicePath);
            }
        }

        currentHueLamps = hueLamps;
    }
    private void OnDiscoveredHueLamp(HueBridge hueBridge, HueLamp hueLamp)
    {
        Debug.Log($"adding light \"{hueLamp.name}\"");

        if (!instantiatedItems.TryGetValue(hueLamp.devicePath, out var item))
        {
            Debug.Log($"creating light item \"{hueLamp.name}\"");

            item = Instantiate(ItemPrefab, Content);
            instantiatedItems[hueLamp.devicePath] = item;

            hueLamp.OnColorUpdate += color =>
            {
                var image = GetColorImage(item);
                image.color = hueLamp.ComputedColor;
            };

            var toggleButton = GetToggleButton(item);
            var toggleButtonText = GetToggleButtonText(item);
            toggleButton.onClick.AddListener(() =>
            {
                hueLamp.on = !hueLamp.on;
                toggleButtonText.text = hueLamp.on ? "On" : "Off";
            });
            toggleButtonText.text = hueLamp.on ? "On" : "Off";

            var brightnessSlider = GetBrightnessSlider(item);
            brightnessSlider.value = hueLamp.Brightness;
            brightnessSlider.onValueChanged.AddListener((value) =>
            {
                hueLamp.Brightness = value;
            });

            var hueSlider = GetHueSlider(item);
            hueSlider.value = hueLamp.Hue;
            hueSlider.onValueChanged.AddListener((value) =>
            {
                hueLamp.Hue = value;
            });

            var saturationSlider = GetSaturationSlider(item);
            saturationSlider.value = hueLamp.Saturation;
            saturationSlider.onValueChanged.AddListener((value) =>
            {
                hueLamp.Saturation = value;
            });
        }
        else
        {
            Debug.Log($"light item \"{hueLamp.name}\" already exists");
        }

        var nameText = item.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();
        nameText.text = hueLamp.name;
    }

    private Button GetToggleButton(GameObject item) => item.transform.Find("Toggle/Button").GetComponent<Button>();
    private TextMeshProUGUI GetToggleButtonText(GameObject item) => item.transform.Find("Toggle/Button").GetComponentInChildren<TextMeshProUGUI>();

    private Slider GetBrightnessSlider(GameObject item) => item.transform.Find("Brightness/Slider").GetComponent<Slider>();
    private Slider GetHueSlider(GameObject item) => item.transform.Find("Hue/Slider").GetComponent<Slider>();
    private Slider GetSaturationSlider(GameObject item) => item.transform.Find("Saturation/Slider").GetComponent<Slider>();

    private Image GetColorImage(GameObject item) => item.transform.Find("Color").GetComponentInChildren<Image>();
}
