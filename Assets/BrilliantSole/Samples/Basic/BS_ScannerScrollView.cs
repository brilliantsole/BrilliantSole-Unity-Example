using System.Collections.Generic;
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

        Text[] textComponents;
        if (instantiatedItems.TryGetValue(DiscoveredDevice.Id, out GameObject item))
        {
            textComponents = item.GetComponentsInChildren<Text>();
        }
        else
        {
            GameObject newItem = Instantiate(ItemPrefab, Content);
            textComponents = newItem.GetComponentsInChildren<Text>();
            instantiatedItems[DiscoveredDevice.Id] = newItem;
        }

        foreach (Text text in textComponents)
        {
            switch (text.name)
            {
                case "Name":
                    text.text = DiscoveredDevice.Name;
                    break;
                case "Rssi":
                    text.text = DiscoveredDevice.Rssi.ToString();
                    break;
            }
        }

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
