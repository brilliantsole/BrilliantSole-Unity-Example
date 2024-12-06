using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_VibrationManager VibrationManager = new();

    private void SetupVibrationManager()
    {
        Managers.Add(VibrationManager);
    }

    public void TriggerVibration(List<BS_VibrationConfiguration> vibrationConfigurations)
    {
        VibrationManager.TriggerVibration(vibrationConfigurations);
    }
}
