using System;

public partial class BS_Device
{
    private readonly BS_BatteryManager BatteryManager = new();
    public delegate void OnBatteryCurrentDelegate(BS_Device device, float batteryCurrent);
    public delegate void OnIsBatteryChargingDelegate(BS_Device device, bool isBatteryCharging);

    public event OnBatteryCurrentDelegate OnBatteryCurrent;
    public event OnIsBatteryChargingDelegate OnIsBatteryCharging;
    private void SetupBatteryManager()
    {
        Managers.Add(BatteryManager);

        BatteryManager.OnBatteryCurrent += batteryCurrent => { OnBatteryCurrent?.Invoke(this, batteryCurrent); };
        BatteryManager.OnIsBatteryCharging += isBatteryCharging => { OnIsBatteryCharging?.Invoke(this, isBatteryCharging); };
    }
}
