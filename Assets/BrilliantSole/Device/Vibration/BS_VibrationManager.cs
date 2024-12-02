using System;
using UnityEngine;
using static BS_VibrationMessageType;

public class BS_VibrationManager : BS_BaseManager<BS_VibrationMessageType>
{
    public static readonly BS_VibrationMessageType[] RequiredMessageTypes = { };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    public override void OnRxMessage(BS_VibrationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            default:
                throw new ArgumentException($"uncaught messageType {messageType}");
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}
