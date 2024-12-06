using System.Collections.Generic;
using UnityEngine;

public partial class BS_DevicePair
{
    private void AddDeviceVibrationListeners(BS_Device device)
    {
        // FILL
    }
    private void RemoveDeviceVibrationListeners(BS_Device device)
    {
        // FILL
    }

    public void TriggerVibration(List<BS_VibrationConfiguration> VibrationConfigurations)
    {
        foreach (var device in devices.Values) { device.TriggerVibration(VibrationConfigurations); }
    }
}
