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
    public static byte[] RequiredTxRxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);
}
