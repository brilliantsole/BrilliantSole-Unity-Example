using System;
using System.Collections.Generic;

public partial class BS_DevicePair
{
    public event Action<BS_DevicePair, BS_Side, BS_Device, bool> OnIsDeviceTfliteReady;
    public event Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceTfliteReady;
    public event Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceTfliteNotReady;
    public event Action<BS_DevicePair, BS_Side, BS_Device, bool> OnIsDeviceTfliteInferencingEnabled;
    public event Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceTfliteInferencingEnabled;
    public event Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceTfliteInferencingDisabled;
    public event Action<BS_DevicePair, BS_Side, BS_Device, List<float>, Dictionary<string, float>, ulong> OnDeviceTfliteInference;
    public event Action<BS_DevicePair, BS_Side, BS_Device, string, float, ulong> OnDeviceTfliteClassification;

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
    private void onDeviceTfliteInference(BS_Device device, List<float> inference, Dictionary<string, float> inferenceMap, ulong timestamp)
    {
        OnDeviceTfliteInference?.Invoke(this, (BS_Side)device.Side, device, inference, inferenceMap, timestamp);
    }
    private void onDeviceTfliteClassification(BS_Device device, string className, float classValue, ulong timestamp)
    {
        OnDeviceTfliteClassification?.Invoke(this, (BS_Side)device.Side, device, className, classValue, timestamp);
    }
}
