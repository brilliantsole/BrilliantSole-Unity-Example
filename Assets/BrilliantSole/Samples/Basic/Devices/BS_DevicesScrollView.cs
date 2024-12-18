using System;
using System.Collections;
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

    public BS_TfliteModelMetadata TfliteModelMetadata;

    private void OnEnable()
    {
        foreach (var device in BS_DeviceManager.AvailableDevices) { OnDevice(device); }
        BS_DeviceManager.OnAvailableDevice += OnDevice;
    }
    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        foreach (var device in BS_DeviceManager.AvailableDevices) { RemoveDevice(device); }
        BS_DeviceManager.OnAvailableDevice -= OnDevice;
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

        device.OnBatteryLevel += OnDeviceBatteryLevel;
        OnDeviceBatteryLevel(device, device.BatteryLevel);

        var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
        toggleConnectionButton.onClick.AddListener(() => { device.ToggleConnection(); });
        device.OnConnectionStatus += OnDeviceConnectionStatus;
        UpdateToggleConnectionButton(device);

        var vibrateButton = item.transform.Find("Vibrate/Button").GetComponent<Button>();
        vibrateButton.onClick.AddListener(() => { device.TriggerVibration(VibrationConfigurations); });

        var toggleTfliteTransfer = item.transform.Find("ToggleTfliteTransfer/Button").GetComponent<Button>();
        toggleTfliteTransfer.onClick.AddListener(() =>
        {
            if (device.FileTransferStatus == BS_FileTransferStatus.Idle)
            {
                if (TfliteModelMetadata != null)
                {
                    Debug.Log("sending tflite model...");
                    device.SendTfliteModel(TfliteModelMetadata);
                }
                else
                {
                    Debug.Log("null TfliteModelMetadata");
                }
            }
            else
            {
                device.CancelFileTransfer();
            }
        });
        device.OnFileTransferStatus += OnDeviceFileTransferStatus;
        OnDeviceFileTransferStatus(device, device.FileTransferStatus);
        device.OnFileTransferProgress += OnDeviceFileTransferProgress;

        device.OnIsTfliteReady += OnIsDeviceTfliteReady;

        var toggleTfliteButton = item.transform.Find("ToggleTflite/Button").GetComponent<Button>();
        toggleTfliteButton.onClick.AddListener(() => { device.ToggleTfliteInferencingEnabled(); });
        device.OnTfliteInferencingEnabled += OnDeviceTfliteInferencingEnabled;
        UpdateToggleTfliteButton(device);

        device.OnTfliteClassification += OnDeviceTfliteClassification;
    }
    private void OnDeviceConnectionStatus(BS_Device device, BS_ConnectionStatus connectionStatus)
    {
        UpdateToggleConnectionButton(device);
    }

    private void UpdateToggleConnectionButton(BS_Device device)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }
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
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        instantiatedItems.Remove(device.Id);

        var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
        toggleConnectionButton.onClick.RemoveAllListeners();

        var vibrateButton = item.transform.Find("Vibrate/Button").GetComponent<Button>();
        vibrateButton.onClick.RemoveAllListeners();

        var toggleTfliteTransferButton = item.transform.Find("ToggleTfliteTransfer/Button").GetComponent<Button>();
        toggleTfliteTransferButton.onClick.RemoveAllListeners();

        device.OnConnectionStatus -= OnDeviceConnectionStatus;
        device.OnFileTransferStatus -= OnDeviceFileTransferStatus;
        device.OnFileTransferProgress -= OnDeviceFileTransferProgress;
        device.OnTfliteInferencingEnabled -= OnDeviceTfliteInferencingEnabled;
        device.OnIsTfliteReady -= OnIsDeviceTfliteReady;
        device.OnTfliteClassification -= OnDeviceTfliteClassification;
        device.OnBatteryLevel -= OnDeviceBatteryLevel;
        Destroy(item);
    }

    private void OnDeviceBatteryLevel(BS_Device device, byte batteryLevel)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        var batteryLevelText = item.transform.Find("BatteryLevel").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating batteryLevel {device.BatteryLevel}");
        batteryLevelText.text = $"Battery: {device.BatteryLevel}%";
    }

    private void OnDeviceFileTransferStatus(BS_Device device, BS_FileTransferStatus fileTransferStatus)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        var toggleTfliteTransferButtonText = item.transform.Find("ToggleTfliteTransfer/Button").GetComponentInChildren<TextMeshProUGUI>();
        toggleTfliteTransferButtonText.text = fileTransferStatus switch
        {
            BS_FileTransferStatus.Idle => "Send Tflite",
            _ => "Cancel"
        };

        var fileTransferProgressContainer = item.transform.Find("FileTransferProgress").gameObject;
        fileTransferProgressContainer.SetActive(fileTransferStatus != BS_FileTransferStatus.Idle);
    }

    private void OnDeviceFileTransferProgress(BS_Device device, BS_FileType fileType, BS_FileTransferDirection fileTransferDirection, float progress)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        var fileTransferProgressText = item.transform.Find("FileTransferProgress").GetComponentInChildren<TextMeshProUGUI>();
        fileTransferProgressText.text = $"{Math.Floor(progress * 100)}%";
    }

    private void UpdateToggleTfliteButton(BS_Device device)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        var toggleTfliteGameObject = item.transform.Find("ToggleTflite").gameObject;
        toggleTfliteGameObject.SetActive(device.IsTfliteReady);

        var toggleTfliteButton = item.transform.Find("ToggleTflite/Button").GetComponent<Button>();
        toggleTfliteButton.interactable = device.IsTfliteReady;

        var toggleTfliteButtonText = item.transform.Find("ToggleTflite/Button").GetComponentInChildren<TextMeshProUGUI>();
        toggleTfliteButtonText.text = device.TfliteInferencingEnabled ? "Disable Tflite" : "Enable Tflite";
    }

    private void OnIsDeviceTfliteReady(BS_Device device, bool isReady)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        UpdateToggleTfliteButton(device);
    }

    private void OnDeviceTfliteInferencingEnabled(BS_Device device, bool isEnabled)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        UpdateToggleTfliteButton(device);

        var tfliteClassificationGameObject = item.transform.Find("TfliteClassification").gameObject;
        tfliteClassificationGameObject.SetActive(isEnabled);
    }

    private void OnDeviceTfliteClassification(BS_Device device, string className, float classValue, ulong timestamp)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { return; }

        var tfliteClassificationText = item.transform.Find("TfliteClassification").GetComponentInChildren<TextMeshProUGUI>();
        tfliteClassificationText.text = $"{className}";

        StartCoroutine(ClearTfliteClassification(device));
    }
    private IEnumerator ClearTfliteClassification(BS_Device device)
    {
        GameObject item = GetItemByDevice(device);
        if (item == null) { yield break; }

        var delay = Math.Max(device.TfliteModelMetadata.CaptureDelay / 1000f, 1f);
        yield return new WaitForSeconds(delay);

        var tfliteClassificationText = item.transform.Find("TfliteClassification").GetComponentInChildren<TextMeshProUGUI>();
        tfliteClassificationText.text = "";
    }

    private GameObject GetItemByDevice(BS_Device device)
    {
        if (!instantiatedItems.ContainsKey(device.Id))
        {
            Debug.LogError($"no item found for device \"{device.Name}\"");
            return null;
        }
        GameObject item = instantiatedItems[device.Id];
        return item;
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
