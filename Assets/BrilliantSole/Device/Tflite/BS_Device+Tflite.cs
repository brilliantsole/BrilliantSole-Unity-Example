using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_TfliteManager TfliteManager = new();
    private void SetupTfliteManager()
    {
        Managers.Add(TfliteManager);

        TfliteManager.OnIsReady += onIsTfliteReady;
        TfliteManager.OnInference += onTfliteInference;
        TfliteManager.OnInferencingEnabled += onTfliteInferencingEnabled;
        TfliteManager.OnClassification += onTfliteClassification;
    }

    public void SendTfliteModel(BS_TfliteModelMetadata tfliteModelMetadata)
    {
        TfliteManager.SendTfliteModel(tfliteModelMetadata, false);
        SendFile(tfliteModelMetadata);
    }

    public bool IsTfliteReady => TfliteManager.IsReady;
    public event Action<BS_Device, bool> OnIsTfliteReady;
    private void onIsTfliteReady(bool isTfliteReady)
    {
        OnIsTfliteReady?.Invoke(this, isTfliteReady);
    }

    public bool TfliteInferencingEnabled => TfliteManager.InferencingEnabled;
    public event Action<BS_Device, bool> OnTfliteInferencingEnabled;
    private void onTfliteInferencingEnabled(bool inferencingEnabled)
    {
        OnTfliteInferencingEnabled?.Invoke(this, inferencingEnabled);
    }

    public event Action<BS_Device, List<float>, Dictionary<string, float>, ulong> OnTfliteInference;
    private void onTfliteInference(List<float> inference, Dictionary<string, float> inferenceMap, ulong timestamp)
    {
        OnTfliteInference?.Invoke(this, inference, inferenceMap, timestamp);
    }

    public event Action<BS_Device, string, float, ulong> OnTfliteClassification;
    private void onTfliteClassification(string className, float classValue, ulong timestamp)
    {
        OnTfliteClassification?.Invoke(this, className, classValue, timestamp);
    }
}
