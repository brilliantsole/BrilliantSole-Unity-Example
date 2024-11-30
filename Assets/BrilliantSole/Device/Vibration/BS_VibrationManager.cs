using UnityEngine;
using static BS_VibrationMessageType;

public class BS_VibrationManager : BS_BaseManager<BS_VibrationMessageType>
{
    public static readonly BS_VibrationMessageType[] RequiredMessageTypes = { };
    public static byte[] RequiredTxRxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);
}
