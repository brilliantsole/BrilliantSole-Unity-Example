using System;

public partial class BS_Device
{
    private readonly BS_BatteryManager BatteryManager = new();
    public event Action<BS_Device, float> OnBatteryCurrent;
    public event Action<BS_Device, bool> OnIsBatteryCharging;
    private void SetupBatteryManager()
    {
        Managers.Add(BatteryManager);

        BatteryManager.OnBatteryCurrent += batteryCurrent => { OnBatteryCurrent?.Invoke(this, batteryCurrent); };
        BatteryManager.OnIsBatteryCharging += isBatteryCharging => { OnIsBatteryCharging?.Invoke(this, isBatteryCharging); };
    }
}
