using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_TfliteManager TfliteManager = new();

    public bool IsTfliteReady => TfliteManager.IsReady && TfliteManager.TfliteModelMetadata != null;
    public bool TfliteInferencingEnabled => IsTfliteReady && TfliteManager.InferencingEnabled;

    public delegate void OnIsTfliteReadyDelegate(BS_Device device, bool isTfliteReady);
    public delegate void OnTfliteReadyDelegate(BS_Device device);
    public delegate void OnTfliteNotReadyDelegate(BS_Device device);
    public delegate void OnIsTfliteInferencingEnabledDelegate(BS_Device device, bool isTfliteInferencingEnabled);
    public delegate void OnTfliteInferencingEnabledDelegate(BS_Device device);
    public delegate void OnTfliteInferencingDisabledDelegate(BS_Device device);
    public delegate void OnTfliteInferenceDelegate(BS_Device device, List<float> confidences, Dictionary<string, float> classificationConfidences, ulong timestamp);
    public delegate void OnTfliteClassificationDelegate(BS_Device device, string classification, float confidence, ulong timestamp);

    public event OnIsTfliteReadyDelegate OnIsTfliteReady;
    public event OnTfliteReadyDelegate OnTfliteReady;
    public event OnTfliteNotReadyDelegate OnTfliteNotReady;
    public event OnIsTfliteInferencingEnabledDelegate OnIsTfliteInferencingEnabled;
    public event OnTfliteInferencingEnabledDelegate OnTfliteInferencingEnabled;
    public event OnTfliteInferencingDisabledDelegate OnTfliteInferencingDisabled;
    public event OnTfliteInferenceDelegate OnTfliteInference;
    public event OnTfliteClassificationDelegate OnTfliteClassification;

    private void SetupTfliteManager()
    {
        Managers.Add(TfliteManager);

        TfliteManager.OnIsReady += onIsTfliteReady;
        TfliteManager.OnInference += onTfliteInference;
        TfliteManager.OnInferencingEnabled += onIsTfliteInferencingEnabled;
        TfliteManager.OnClassification += onTfliteClassification;
    }

    public async void SendTfliteModel(BS_TfliteModelMetadata tfliteModelMetadata)
    {
        TfliteManager.SendTfliteModel(tfliteModelMetadata);
        var isSendingFile = await SendFile(tfliteModelMetadata);
        if (!isSendingFile)
        {
            onIsTfliteReady(IsTfliteReady);
        }
    }
    public void SetTfliteInferencingEnabled(bool inferencingEnabled, bool sendImmediately = true) { TfliteManager.SetInferencingEnabled(inferencingEnabled, sendImmediately); }
    public void ToggleTfliteInferencingEnabled() { TfliteManager.ToggleInferencingEnabled(); }

    public BS_TfliteModelMetadata TfliteModelMetadata => TfliteManager.TfliteModelMetadata;

    private void onIsTfliteReady(bool isTfliteReady)
    {
        OnIsTfliteReady?.Invoke(this, IsTfliteReady);
        if (IsTfliteReady)
        {
            OnTfliteReady?.Invoke(this);
        }
        else
        {
            OnTfliteNotReady?.Invoke(this);
        }
    }
    private void onIsTfliteInferencingEnabled(bool inferencingEnabled)
    {
        OnIsTfliteInferencingEnabled?.Invoke(this, TfliteInferencingEnabled);
        if (TfliteInferencingEnabled)
        {
            OnTfliteInferencingEnabled?.Invoke(this);
        }
        else
        {
            OnTfliteInferencingDisabled?.Invoke(this);
        }
    }
    private void onTfliteInference(List<float> inference, Dictionary<string, float> inferenceMap, ulong timestamp)
    {
        OnTfliteInference?.Invoke(this, inference, inferenceMap, timestamp);
    }
    private void onTfliteClassification(string className, float classValue, ulong timestamp)
    {
        OnTfliteClassification?.Invoke(this, className, classValue, timestamp);
    }
}
