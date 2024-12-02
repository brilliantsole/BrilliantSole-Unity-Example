using System;

public partial class BS_Device
{
    private readonly BS_TfliteManager TfliteManager = new();
    private void SetupTfliteManager()
    {
        Managers.Add(TfliteManager);

        // FILL
    }
}
