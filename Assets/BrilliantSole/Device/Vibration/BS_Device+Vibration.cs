using System;
using System.Collections.Generic;

public partial class BS_Device
{
    private readonly BS_VibrationManager VibrationManager = new();

    public event Action<BS_Device, BS_VibrationLocation[]> OnVibrationLocations;


    private void SetupVibrationManager()
    {
        Managers.Add(VibrationManager);

        VibrationManager.OnVibrationLocations += vibrationLocations => OnVibrationLocations?.Invoke(this, vibrationLocations);
    }

    public void TriggerVibration(List<BS_VibrationConfiguration> vibrationConfigurations)
    {
        VibrationManager.TriggerVibration(vibrationConfigurations);
    }
}
