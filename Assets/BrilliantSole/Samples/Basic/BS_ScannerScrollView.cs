using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BS_ScannerScrollView : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private readonly Dictionary<string, GameObject> instantiatedItems = new();

    private void OnEnable()
    {
        foreach (BS_DiscoveredDevice DiscoveredDevice in BS_ScannerManager.Instance.DiscoveredDevices.Values)
        {
            OnDiscoveredDevice(DiscoveredDevice);
        }
    }
    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        foreach (BS_DiscoveredDevice DiscoveredDevice in BS_ScannerManager.Instance.DiscoveredDevices.Values)
        {
            OnExpiredDevice(DiscoveredDevice);
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

            TextMeshProUGUI nameText = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            nameText.text = DiscoveredDevice.Name;


            Button toggleConnectionButton = item.transform.Find("ToggleConnection").GetComponent<Button>();
            TextMeshProUGUI toggleConnectionButtonText = item.transform.Find("ToggleConnection").GetComponentInChildren<TextMeshProUGUI>();
            toggleConnectionButton.onClick.AddListener(() =>
            {
                // FILL - find device in deviceManager if it exists
                // FILL - disconnect if it doesn't exist
                toggleConnectionButtonText.text = "Connecting...";
                Debug.Log($"Connecting to \"{DiscoveredDevice.Name}\"");
                BS_ScannerManager.Instance.ConnectToDiscoveredDevice(DiscoveredDevice);
            });
        }

        TextMeshProUGUI rssiText = item.transform.Find("Rssi").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating rssi text to {DiscoveredDevice.Rssi}");
        rssiText.text = DiscoveredDevice.Rssi.ToString();

        TextMeshProUGUI deviceTypeText = item.transform.Find("DeviceType").GetComponent<TextMeshProUGUI>();
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
            Button toggleConnectionButton = item.transform.Find("ToggleConnection").GetComponent<Button>();
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
