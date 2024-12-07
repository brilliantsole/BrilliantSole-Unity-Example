using System;
using UnityEngine;
using static BS_TfliteMessageType;

public class BS_TfliteManager : BS_BaseManager<BS_TfliteMessageType>
{
    public static readonly BS_TfliteMessageType[] RequiredMessageTypes = {
        GetTfliteName,
        GetTfliteTask,
        GetTfliteSampleRate,
        GetTfliteSensorTypes,
        IsTfliteReady,
        GetTfliteCaptureDelay,
        GetTfliteThreshold,
        GetTfliteInferencingEnabled,
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TfliteManager");

    public override void OnRxMessage(BS_TfliteMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            // FILL
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public override void Reset()
    {
        base.Reset();
        // FILL
    }
}
