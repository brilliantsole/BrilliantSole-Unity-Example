using System;

public partial class BS_Device
{
    private readonly BS_VibrationManager VibrationManager = new();

    private void SetupVibrationManager()
    {
        Managers.Add(VibrationManager);

        // FILL
    }
}
