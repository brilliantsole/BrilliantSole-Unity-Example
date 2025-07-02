using System.Collections.Generic;
using System;
using UnityEngine;

public class BS_VibrationManager : BS_BaseManager<BS_VibrationMessageType>
{
    public static readonly BS_VibrationMessageType[] RequiredMessageTypes = {
        BS_VibrationMessageType.GetVibrationLocations
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_VibrationManager", BS_Logger.LogLevel.Log);

    public event Action<BS_VibrationLocation[]> OnVibrationLocations;


    public override void OnRxMessage(BS_VibrationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case BS_VibrationMessageType.GetVibrationLocations:
                ParseVibrationLocations(data);
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    // FILE TYPES START
    [SerializeField]
    private BS_VibrationLocation[] _vibrationLocations = Array.Empty<BS_VibrationLocation>();
    public BS_VibrationLocation[] VibrationLocations
    {
        get => _vibrationLocations;
        private set
        {
            Logger.Log($"Updating VibrationLocations to {value}");
            _vibrationLocations = value;
            OnVibrationLocations?.Invoke(VibrationLocations);
        }
    }
    private void ParseVibrationLocations(in byte[] data)
    {
        BS_VibrationLocation[] vibrationLocations = new BS_VibrationLocation[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            vibrationLocations[i] = (BS_VibrationLocation)data[i];
        }
        Logger.Log($"Parsed vibrationLocations: {string.Join(", ", vibrationLocations)}");
        VibrationLocations = vibrationLocations;
    }
    // FILE TYPES END

    public override void Reset()
    {
        base.Reset();

        _vibrationLocations = Array.Empty<BS_VibrationLocation>();
    }

    public void TriggerVibration(List<BS_VibrationConfiguration> vibrationConfigurations)
    {
        Logger.Log("triggering vibration...");
        List<byte> TxData = new();
        for (int i = 0; i < vibrationConfigurations.Count; i++)
        {
            var array = vibrationConfigurations[i].ToArray();
            if (array.Count == 0)
            {
                Logger.Log("empty vibrationConfiguration array - skipping");
                continue;
            }
            TxData.AddRange(array);
        }
        if (TxData.Count == 0)
        {
            Logger.Log("empty vibration TxData - not sending");
            return;
        }
        BS_TxMessage[] Messages = { CreateMessage(BS_VibrationMessageType.TriggerVibration, TxData) };
        SendTxMessages?.Invoke(Messages, true);
    }
}
