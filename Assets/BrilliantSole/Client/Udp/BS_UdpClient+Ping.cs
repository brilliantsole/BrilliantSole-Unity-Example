using System.Collections;
using System.Timers;
using UnityEngine;

public partial class BS_UdpClient
{
    private static readonly double PingInterval = 1000;
    private Timer _pingTimer;
    private Timer PingTimer
    {
        get
        {
            if (_pingTimer == null)
            {
                _pingTimer = new(PingInterval) { AutoReset = true };
                _pingTimer.Elapsed += (sender, e) => Ping();
            }
            return _pingTimer;
        }
    }

    private void Ping()
    {
        Logger.Log("Pinging server");
        // FILL
    }
}
