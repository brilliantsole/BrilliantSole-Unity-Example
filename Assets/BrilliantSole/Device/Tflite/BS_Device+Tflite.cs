using System;

public partial class BS_Device
{
    private readonly BS_TfliteManager TfliteManager = new();
    private void SetupTfliteManager()
    {
        Managers.Add(TfliteManager);

        // FILL
    }

    public void SendTfliteModel(BS_TfliteModelMetadata tfliteModelMetadata)
    {
        TfliteManager.SendTfliteModel(tfliteModelMetadata, false);
        SendFile(tfliteModelMetadata);
    }
}
