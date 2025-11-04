using System.Collections.Generic;

public partial class BS_DevicePair
{
    public delegate void OnIsDeviceTfliteReadyDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        bool isTfliteReady
    );
    public delegate void OnIsDeviceTfliteInferencingEnabledDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        bool isTfliteInferencingEnabled
    );
    public delegate void OnDeviceTfliteInferenceDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        List<float> confidences,
        Dictionary<string, float> classificationConfidences,
        ulong timestamp
    );
    public delegate void OnDeviceTfliteClassificationDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        string classification,
        float confidence,
        ulong timestamp
    );

    public event OnIsDeviceTfliteReadyDelegate OnIsDeviceTfliteReady;
    public event OnDeviceDelegate OnDeviceTfliteReady;
    public event OnDeviceDelegate OnDeviceTfliteNotReady;
    public event OnIsDeviceTfliteInferencingEnabledDelegate OnIsDeviceTfliteInferencingEnabled;
    public event OnDeviceDelegate OnDeviceTfliteInferencingEnabled;
    public event OnDeviceDelegate OnDeviceTfliteInferencingDisabled;
    public event OnDeviceTfliteInferenceDelegate OnDeviceTfliteInference;
    public event OnDeviceTfliteClassificationDelegate OnDeviceTfliteClassification;
    private void AddDeviceTfliteListeners(BS_Device device)
    {
        device.OnIsTfliteReady += onDeviceIsTfliteReady;
        device.OnIsTfliteInferencingEnabled += onIsDeviceTfliteInferencingEnabled;
        device.OnTfliteInference += onDeviceTfliteInference;
        device.OnTfliteClassification += onDeviceTfliteClassification;
    }
    private void RemoveDeviceTfliteListeners(BS_Device device)
    {
        device.OnIsTfliteReady -= onDeviceIsTfliteReady;
        device.OnIsTfliteInferencingEnabled -= onIsDeviceTfliteInferencingEnabled;
        device.OnTfliteInference -= onDeviceTfliteInference;
        device.OnTfliteClassification -= onDeviceTfliteClassification;
    }

    public void SetTfliteInferencingEnabled(bool inferencingEnabled, bool sendImmediately = true)
    {
        foreach (var device in devices.Values) { device.SetTfliteInferencingEnabled(inferencingEnabled, sendImmediately); }
    }

    public async void SendTfliteModel(BS_TfliteModelMetadata tfliteModelMetadata)
    {
        foreach (var device in devices.Values) { device.SendTfliteModel(tfliteModelMetadata); }
    }

    private void onDeviceIsTfliteReady(BS_Device device, bool isTfliteReady)
    {
        OnIsDeviceTfliteReady?.Invoke(this, (BS_Side)device.Side, device, isTfliteReady);
        if (isTfliteReady)
        {
            OnDeviceTfliteReady?.Invoke(this, (BS_Side)device.Side, device);
        }
        else
        {
            OnDeviceTfliteNotReady?.Invoke(this, (BS_Side)device.Side, device);
        }
    }
    private void onIsDeviceTfliteInferencingEnabled(BS_Device device, bool isTfliteInferencingEnabled)
    {
        OnIsDeviceTfliteInferencingEnabled?.Invoke(this, (BS_Side)device.Side, device, isTfliteInferencingEnabled);
        if (isTfliteInferencingEnabled)
        {
            OnDeviceTfliteInferencingEnabled?.Invoke(this, (BS_Side)device.Side, device);
        }
        else
        {
            OnDeviceTfliteInferencingDisabled?.Invoke(this, (BS_Side)device.Side, device);
        }
    }
    private void onDeviceTfliteInference(BS_Device device, List<float> confidences, Dictionary<string, float> confidenceMap, ulong timestamp)
    {
        OnDeviceTfliteInference?.Invoke(this, (BS_Side)device.Side, device, confidences, confidenceMap, timestamp);
    }
    private void onDeviceTfliteClassification(BS_Device device, string classification, float confidence, ulong timestamp)
    {
        OnDeviceTfliteClassification?.Invoke(this, (BS_Side)device.Side, device, classification, confidence, timestamp);
    }
}
