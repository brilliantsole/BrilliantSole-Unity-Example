using System;
using UnityEngine;

public partial class BS_Device
{
    public event Action<BS_Device, byte> OnBatteryLevel;

    [SerializeField]
    private byte _batteryLevel;
    public byte BatteryLevel
    {
        get => _batteryLevel;
        private set
        {
            if (_batteryLevel == value) { return; }
            Logger.Log($"Updating BatteryLevel to {value}");
            _batteryLevel = value;
            OnBatteryLevel?.Invoke(this, _batteryLevel);
        }
    }
}