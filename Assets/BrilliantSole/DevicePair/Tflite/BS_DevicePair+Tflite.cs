using System;
using System.Collections.Generic;

public partial class BS_DevicePair
{
    public event Action<BS_DevicePair, BS_Side, BS_Device, bool> OnIsDeviceTfliteReady;
    public event Action<BS_DevicePair, BS_Side, BS_Device, bool> OnDeviceTfliteInferencingEnabled;
    public event Action<BS_DevicePair, BS_Side, BS_Device, List<float>, Dictionary<string, float>, ulong> OnDeviceTfliteInference;
    public event Action<BS_DevicePair, BS_Side, BS_Device, string, float, ulong> OnDeviceTfliteClassification;

    private void AddDeviceTfliteListeners(BS_Device device)
    {
        device.OnIsTfliteReady += onDeviceIsTfliteReady;
        device.OnTfliteInferencingEnabled += onDeviceTfliteInferencingEnabled;
        device.OnTfliteInference += onDeviceTfliteInference;
        device.OnTfliteClassification += onDeviceTfliteClassification;
    }
    private void RemoveDeviceTfliteListeners(BS_Device device)
    {
        device.OnIsTfliteReady -= onDeviceIsTfliteReady;
        device.OnTfliteInferencingEnabled -= onDeviceTfliteInferencingEnabled;
        device.OnTfliteInference -= onDeviceTfliteInference;
        device.OnTfliteClassification -= onDeviceTfliteClassification;
    }

    public void SetTfliteInferencingEnabled(bool inferencingEnabled, bool sendImmediately = true)
    {
        foreach (var device in devices.Values) { device.SetTfliteInferencingEnabled(inferencingEnabled, sendImmediately); }
    }

    private void onDeviceIsTfliteReady(BS_Device device, bool isTfliteReady)
    {
        OnIsDeviceTfliteReady?.Invoke(this, (BS_Side)device.Side, device, isTfliteReady);
    }
    private void onDeviceTfliteInferencingEnabled(BS_Device device, bool isTfliteInferencingEnabled)
    {
        OnDeviceTfliteInferencingEnabled?.Invoke(this, (BS_Side)device.Side, device, isTfliteInferencingEnabled);
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
