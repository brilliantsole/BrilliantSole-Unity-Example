using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static BS_ConnectionStatus;

public class BS_DevicesScrollView : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private readonly Dictionary<string, GameObject> instantiatedItems = new();

    public List<BS_VibrationConfiguration> VibrationConfigurations = new();

    private void OnEnable()
    {
        foreach (var device in BS_DeviceManager.AvailableDevices)
        {
            OnDevice(device);
        }
    }
    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        foreach (var device in BS_DeviceManager.AvailableDevices)
        {
            RemoveDevice(device);
        }
    }

    public void OnDevice(BS_Device device)
    {
        Debug.Log($"updating item for \"{device.Name}\"");

        GameObject item;
        if (instantiatedItems.ContainsKey(device.Id)) { return; }

        item = Instantiate(ItemPrefab, Content);
        instantiatedItems[device.Id] = item;

        var nameText = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        nameText.text = device.Name;

        var deviceTypeText = item.transform.Find("DeviceType").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating deviceType {device.DeviceType}");
        deviceTypeText.text = device.DeviceType.ToString();

        var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
        toggleConnectionButton.onClick.AddListener(() => { device.ToggleConnection(); });

        var vibrateButton = item.transform.Find("Vibrate/Button").GetComponent<Button>();
        vibrateButton.onClick.AddListener(() => { device.TriggerVibration(VibrationConfigurations); });

        device.OnConnectionStatus += OnDeviceConnectionStatus;
        UpdateToggleConnectionButton(device);
    }

    private void OnDeviceConnectionStatus(BS_Device device, BS_ConnectionStatus connectionStatus)
    {
        UpdateToggleConnectionButton(device);
    }

    private void UpdateToggleConnectionButton(BS_Device device)
    {
        if (!instantiatedItems.ContainsKey(device.Id))
        {
            Debug.LogError($"no item found for device \"{device.Name}\"");
            return;
        }
        GameObject item = instantiatedItems[device.Id];
        var toggleConnectionButtonText = item.transform.Find("ToggleConnection/Button").GetComponentInChildren<TextMeshProUGUI>();
        toggleConnectionButtonText.text = device.ConnectionStatus switch
        {
            NotConnected => "Connect",
            Connecting => "Connecting",
            Connected => "Disconnect",
            Disconnecting => "Disconnecting",
            _ => ""
        };
    }

    public void RemoveDevice(BS_Device device)
    {
        if (instantiatedItems.TryGetValue(device.Id, out GameObject item))
        {
            instantiatedItems.Remove(device.Id);
            var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
            toggleConnectionButton.onClick.RemoveAllListeners();
            var vibrateButton = item.transform.Find("Vibrate/Button").GetComponent<Button>();
            vibrateButton.onClick.RemoveAllListeners();
            device.OnConnectionStatus -= OnDeviceConnectionStatus;
            Destroy(item);
        }
    }


    public void Clear()
    {
        foreach (var item in instantiatedItems.Values)
        {
            Destroy(item);
        }
        instantiatedItems.Clear();
    }
}
