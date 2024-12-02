using System;
using UnityEngine;
using static BS_VibrationMessageType;

public class BS_VibrationManager : BS_BaseManager<BS_VibrationMessageType>
{
    public static readonly BS_VibrationMessageType[] RequiredMessageTypes = { };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_VibrationManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_VibrationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}
