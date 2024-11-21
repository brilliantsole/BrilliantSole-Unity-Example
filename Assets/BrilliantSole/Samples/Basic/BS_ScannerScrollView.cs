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

            // FILL - button 
        }

        TextMeshProUGUI rssiText = item.transform.Find("Rssi").GetComponent<TextMeshProUGUI>();
        Debug.Log($"updating rssi text to {DiscoveredDevice.Rssi}");
        rssiText.text = DiscoveredDevice.Rssi.ToString();

    }
    public void OnExpiredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        if (instantiatedItems.TryGetValue(DiscoveredDevice.Id, out GameObject item))
        {
            instantiatedItems.Remove(DiscoveredDevice.Id);
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
