using UnityEngine;
using static BS_SensorDataMessageType;

public class BS_SensorDataManager : BS_BaseManager<BS_SensorDataMessageType>
{
    public static readonly BS_SensorDataMessageType[] RequiredMessageTypes = {
        GetPressurePositions,
        GetSensorScalars
     };
    public static byte[] RequiredTxRxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);
}
