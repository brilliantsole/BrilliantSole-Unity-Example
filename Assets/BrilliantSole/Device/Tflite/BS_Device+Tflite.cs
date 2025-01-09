using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_TfliteManager TfliteManager = new();

    public bool IsTfliteReady => TfliteManager.IsReady && TfliteManager.TfliteModelMetadata != null;
    public bool TfliteInferencingEnabled => TfliteManager.InferencingEnabled;

    public event Action<BS_Device, bool> OnIsTfliteReady;
    public event Action<BS_Device, bool> OnTfliteInferencingEnabled;
    public event Action<BS_Device, List<float>, Dictionary<string, float>, ulong> OnTfliteInference;
    public event Action<BS_Device, string, float, ulong> OnTfliteClassification;

    private void SetupTfliteManager()
    {
        Managers.Add(TfliteManager);

        TfliteManager.OnIsReady += onIsTfliteReady;
        TfliteManager.OnInference += onTfliteInference;
        TfliteManager.OnInferencingEnabled += onTfliteInferencingEnabled;
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
        OnIsTfliteReady?.Invoke(this, isTfliteReady);
    }
    private void onTfliteInferencingEnabled(bool inferencingEnabled)
    {
        OnTfliteInferencingEnabled?.Invoke(this, inferencingEnabled);
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
