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

    void Start()
    {
        hueBridge = HueBridge.GetComponent<HueBridge>();
        OnDiscoveredHueLamps(hueBridge, hueBridge.HueLamps);
        hueBridge.OnDiscoveredHueLamps += OnDiscoveredHueLamps;
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

                Destroy(item);
                instantiatedItems.Remove(hueLamp.devicePath);
            }
        }

        currentHueLamps = hueLamps;
    }
    private void OnDiscoveredHueLamp(HueBridge hueBridge, HueLamp hueLamp)
    {
        Debug.Log($"adding light {hueLamp.devicePath}");

        if (!instantiatedItems.TryGetValue(hueLamp.devicePath, out var item))
        {
            item = Instantiate(ItemPrefab, Content);
            instantiatedItems[hueLamp.devicePath] = item;

            var toggleButton = GetToggleButton(item);
            var toggleButtonText = GetToggleButtonText(item);
            toggleButton.onClick.AddListener(() =>
            {
                hueLamp.on = !hueLamp.on;
                toggleButtonText.text = hueLamp.on ? "On" : "Off";
            });
            toggleButton.onClick.RemoveAllListeners();

            var brightnessSlider = GetBrightnessSlider(item);
            brightnessSlider.onValueChanged.AddListener((value) =>
            {
                hueLamp.hsv.z = value;
                hueLamp.shouldUpdateHSV = true;
            });
        }

        var nameText = item.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();
        nameText.text = hueLamp.name;
    }

    private Button GetToggleButton(GameObject item) => item.transform.Find("Toggle/Button").GetComponent<Button>();
    private TextMeshProUGUI GetToggleButtonText(GameObject item) => item.transform.Find("Toggle/Button").GetComponentInChildren<TextMeshProUGUI>();

    private Slider GetBrightnessSlider(GameObject item) => item.transform.Find("Brightness/Slider").GetComponent<Slider>();
}
