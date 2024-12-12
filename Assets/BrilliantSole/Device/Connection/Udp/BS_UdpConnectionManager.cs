using UnityEngine;

public class BS_UdpConnectionManager : BS_BaseConnectionManager
{
    public override BS_ConnectionType Type => BS_ConnectionType.Udp;

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }
        // FILL
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }
        // FILL
    }
}
