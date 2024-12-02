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

    public override void OnRxMessage(BS_TfliteMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            // FILL
            default:
                throw new ArgumentException($"uncaught messageType {messageType}");
        }
    }

    public override void Reset()
    {
        base.Reset();
        // FILL
    }
}
