using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BS_ConnectionStatus;

public class BS_UdpClientScrollView : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private readonly Dictionary<string, GameObject> instantiatedItems = new();
    private BS_UdpClientManager UdpClientManager => BS_UdpClientManager.Instance;

    private void OnEnable()
    {
        //foreach (var DiscoveredDevice in ScannerManager.DiscoveredDevices.Values) { OnDiscoveredDevice(DiscoveredDevice); }
        // FILL - add listeners
    }
    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        //foreach (var DiscoveredDevice in ScannerManager.DiscoveredDevices.Values) { OnExpiredDevice(DiscoveredDevice); }
        // FILL - remove listeners
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
            BS_Device device = null;
            // FILL - check if UdpClientManager already has it
            toggleConnectionButton.onClick.AddListener(() =>
            {
                Debug.Log($"Toggling Connection to \"{DiscoveredDevice.Name}\"...");

                // FIX
                //var _device = ScannerManager.ToggleConnectionToDiscoveredDevice(DiscoveredDevice);
                BS_Device _device = null;
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