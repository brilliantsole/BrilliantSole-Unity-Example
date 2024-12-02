using System;
using System.Text;
using UnityEngine;
using static BS_BatteryMessageType;

public class BS_BatteryManager : BS_BaseManager<BS_BatteryMessageType>
{
    public static readonly BS_BatteryMessageType[] RequiredMessageTypes = {
        GetIsBatteryCharging,
        GetBatteryCurrent
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BatteryManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_BatteryMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetIsBatteryCharging:
                ParseIsBatteryCharging(data);
                break;
            case GetBatteryCurrent:
                ParseBatteryCurrent(data);
                break;
            default:
                throw new ArgumentException($"uncaught messageType {messageType}");
        }
    }



    [SerializeField]
    private bool? _isBatteryCharging;
    public bool IsBatteryCharging
    {
        get => _isBatteryCharging ?? false;
        private set
        {
            if (_isBatteryCharging == value) { return; }
            Logger.Log($"Updating IsBatteryCharging to {value}");
            _isBatteryCharging = value;
            OnIsBatteryCharging?.Invoke(IsBatteryCharging);
        }
    }
    public event Action<bool> OnIsBatteryCharging;
    private void ParseIsBatteryCharging(in byte[] data)
    {
        bool isBatteryCharging = data[0] == 1;
        Logger.Log($"parsed isBatteryCharging: {isBatteryCharging}");
        IsBatteryCharging = isBatteryCharging;
    }

    [SerializeField]
    private float? _batteryCurrent;
    public float BatteryCurrent
    {
        get => _batteryCurrent ?? 0.0f;
        private set
        {
            if (_batteryCurrent == value) { return; }
            Logger.Log($"Updating BatteryCurrent to {value}");
            _batteryCurrent = value;
            OnBatteryCurrent?.Invoke(BatteryCurrent);
        }
    }
    public event Action<float> OnBatteryCurrent;
    private void ParseBatteryCurrent(in byte[] data)
    {
        var batteryCurrent = BS_ByteUtils.ParseNumber<float>(data, isLittleEndian: true);
        Logger.Log($"parsed batteryCurrent: {batteryCurrent}");
        BatteryCurrent = batteryCurrent;
    }

    public override void Reset()
    {
        base.Reset();

        _isBatteryCharging = null;
        _batteryCurrent = null;
    }
}
