using UnityEngine;
using static BS_InformationMessageType;

public class BS_InformationManager : BS_BaseManager<BS_InformationMessageType>
{
    public static readonly BS_InformationMessageType[] RequiredMessageTypes = {
        IsBatteryCharging,
        GetBatteryCurrent,
        GetMtu,
        GetId,
        GetName,
        BS_InformationMessageType.GetType,
        GetCurrentTime
     };
    public static byte[] RequiredTxRxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_InformationManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_InformationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            // FILL
            case GetMtu:
                ParseMtu(data);
                break;
        }
    }

    public ushort Mtu { get; private set; }
    public ushort MaxTxMessageLength => (ushort)(Mtu == 0 ? 0 : Mtu - 3);
    private void ParseMtu(in byte[] data)
    {
        ushort mtu = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"Parsed mtu: {mtu}");
    }
}
