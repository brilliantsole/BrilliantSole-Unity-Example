using UnityEngine;

public class BS_BleConnectionManager : BS_BaseConnectionManager
{
    public override BS_ConnectionType Type => BS_ConnectionType.Ble;

    protected override void Connect(in bool Continue)
    {
        base.Connect(Continue);
        if (!Continue)
        {
            return;
        }
        // FILL
    }
    protected override void Disconnect(in bool Continue)
    {
        base.Disconnect(Continue);
        if (!Continue)
        {
            return;
        }
        // FILL
    }
}
