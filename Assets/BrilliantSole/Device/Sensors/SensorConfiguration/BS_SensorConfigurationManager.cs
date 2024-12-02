using System;
using static BS_SensorConfigurationMessageType;

public class BS_SensorConfigurationManager : BS_BaseManager<BS_SensorConfigurationMessageType>
{
    public static readonly BS_SensorConfigurationMessageType[] RequiredMessageTypes = {
        GetSensorConfiguration
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    public override void OnRxMessage(BS_SensorConfigurationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetSensorConfiguration:
            case SetSensorConfiguration:
                // FILL
                break;
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
