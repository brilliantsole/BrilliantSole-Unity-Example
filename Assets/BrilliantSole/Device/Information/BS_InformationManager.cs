using UnityEngine;
using static BS_InformationMessageType;

public class BS_InformationManager : BS_BaseManager<BS_InformationMessageType>
{
    public static readonly BS_InformationMessageType[] RequiredMessageTypes = {
        GetMtu
     };
    public static byte[] RequiredTxMessageTypes => ConvertEnumToTxRx(RequiredMessageTypes);

    public ushort Mtu { get; private set; }
    public ushort MaxTxMessageLength => (ushort)(Mtu == 0 ? 0 : Mtu - 3);
}
