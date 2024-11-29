using UnityEngine;

public class BS_InformationManager : BS_BaseManager<BS_InformationMessageType>
{
    public ushort Mtu { get; private set; }
    public ushort MaxTxMessageLength => (ushort)(Mtu == 0 ? 0 : Mtu - 3);
}
