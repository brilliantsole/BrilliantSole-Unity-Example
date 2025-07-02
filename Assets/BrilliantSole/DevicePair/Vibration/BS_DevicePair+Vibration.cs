using System;
using System.Collections.Generic;

public partial class BS_DevicePair
{
    public event Action<BS_DevicePair, BS_Side, BS_Device, BS_VibrationLocation[]> OnDeviceVibrationLocations;

    private void AddDeviceVibrationListeners(BS_Device device)
    {
        device.OnVibrationLocations += onDeviceVibrationLocations;
    }
    private void RemoveDeviceVibrationListeners(BS_Device device)
    {
        device.OnVibrationLocations -= onDeviceVibrationLocations;
    }

    private void onDeviceVibrationLocations(BS_Device device, BS_VibrationLocation[] vibrationLocations)
    {
        OnDeviceVibrationLocations?.Invoke(this, (BS_Side)device.Side, device, vibrationLocations);
    }

    public void TriggerVibration(List<BS_VibrationConfiguration> VibrationConfigurations)
    {
        foreach (var device in devices.Values) { device.TriggerVibration(VibrationConfigurations); }
    }
}
