using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BS_ConnectionStatus;

public abstract class BS_BaseScannerScrollView : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private readonly Dictionary<string, GameObject> instantiatedItems = new();
    public Button ToggleScanButton;
    protected abstract IBS_ScannerManager ScannerManager { get; }

    protected virtual void OnEnable()
    {
        ToggleScanButton.onClick.AddListener(ScannerManager.ToggleScan);
        ScannerManager.OnIsScanning.AddListener(OnIsScanning);
        ScannerManager.OnIsScanningAvailable.AddListener(OnIsScanningAvailable);

        ScannerManager.OnDiscoveredDevice.AddListener(OnDiscoveredDevice);
        ScannerManager.OnExpiredDevice.AddListener(OnExpiredDevice);

        foreach (var DiscoveredDevice in ScannerManager.DiscoveredDevices.Values) { OnDiscoveredDevice(DiscoveredDevice); }
    }
    protected virtual void OnDisable()
    {
        ToggleScanButton.onClick.RemoveListener(ScannerManager.ToggleScan);
        ScannerManager.OnIsScanning.RemoveListener(OnIsScanning);
        ScannerManager.OnIsScanningAvailable.RemoveListener(OnIsScanningAvailable);

        ScannerManager.OnDiscoveredDevice.RemoveListener(OnDiscoveredDevice);
        ScannerManager.OnExpiredDevice.RemoveListener(OnExpiredDevice);

        if (!gameObject.scene.isLoaded) return;
        //foreach (var DiscoveredDevice in ScannerManager.DiscoveredDevices.Values) { OnExpiredDevice(DiscoveredDevice); }
    }

    private void OnIsScanningAvailable(IBS_ScannerManager scannerManager, bool IsScanningAvailable)
    {
        ToggleScanButton.interactable = IsScanningAvailable;
    }
    private void OnIsScanning(IBS_ScannerManager scannerManager, bool isScanning)
    {
        var toggleScanButtonText = ToggleScanButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        toggleScanButtonText.text = isScanning ? "Stop Scan" : "Start Scan";
        if (isScanning)
        {
            Clear();
        }
    }

    public void OnDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        Debug.Log($"updating item for \"{DiscoveredDevice.Name}\"");

        GameObject item;
        if (instantiatedItems.ContainsKey(DiscoveredDevice.Id))
        {
            item = instantiatedItems[DiscoveredDevice.Id];
        }
        else
        {
            item = Instantiate(ItemPrefab, Content);
            instantiatedItems[DiscoveredDevice.Id] = item;

            var nameText = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            nameText.text = DiscoveredDevice.Name;

            var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
            var toggleConnectionButtonText = toggleConnectionButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            BS_Device device = DiscoveredDevice.Device;
            if (device != null)
            {
                toggleConnectionButtonText.text = device.ConnectionStatus switch
                {
                    NotConnected => "Connect",
                    Connecting => "Connecting",
                    Connected => "Disconnect",
                    Disconnecting => "Disconnecting",
                    _ => throw new System.NotImplementedException()
                };
            }
            toggleConnectionButton.onClick.AddListener(() =>
            {
                Debug.Log($"Toggling Connection to \"{DiscoveredDevice.Name}\"...");

                var _device = DiscoveredDevice.ToggleConnection();
                if (device == null)
                {
                    Debug.Log("first time connecting to device...");
                    device = _device;
                    device.OnConnectionStatus += (device, connectionStatus) =>
                    {
                        Debug.Log($"device \"{device.Name}\" updated connectionStatus to {connectionStatus}");
                        toggleConnectionButtonText.text = connectionStatus switch
                        {
                            NotConnected => "Connect",
                            Connecting => "Connecting",
                            Connected => "Disconnect",
                            Disconnecting => "Disconnecting",
                            _ => throw new System.NotImplementedException()
                        };
                    };
                    toggleConnectionButtonText.text = "Connecting...";
                }
            });
        }

        var rssiText = item.transform.Find("Rssi").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating rssi text to {DiscoveredDevice.Rssi}");
        rssiText.text = $"Rssi: {DiscoveredDevice.Rssi}";

        var deviceTypeText = item.transform.Find("DeviceType").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating deviceType {DiscoveredDevice.DeviceType}");
        if (DiscoveredDevice.DeviceType != null)
        {
            deviceTypeText.text = DiscoveredDevice.DeviceType.ToString();
        }
        else
        {
            deviceTypeText.text = "";
        }
    }
    public void OnExpiredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        if (instantiatedItems.TryGetValue(DiscoveredDevice.Id, out GameObject item))
        {
            instantiatedItems.Remove(DiscoveredDevice.Id);
            var toggleConnectionButton = item.transform.Find("ToggleConnection/Button").GetComponent<Button>();
            toggleConnectionButton.onClick.RemoveAllListeners();
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
