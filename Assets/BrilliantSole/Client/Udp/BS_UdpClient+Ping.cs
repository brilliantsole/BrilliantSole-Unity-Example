using System.Timers;

public partial class BS_UdpClient
{
    private static readonly double PingInterval = 2000;
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

    private bool DidSetRemoteReceivePort = false;
    static private readonly BS_UdpMessage PingMessage = new(BS_UdpMessageType.Ping);
    private void Ping()
    {
        BS_UdpMessage message;
        if (DidSetRemoteReceivePort)
        {
            Logger.Log("Pinging server");
            message = PingMessage;
        }
        else
        {
            Logger.Log("setting remote receive port");
            message = new(BS_UdpMessageType.SetRemoteReceivePort, BS_ByteUtils.ToByteArray((ushort)ReceivePort));
        }
        SendUdpMessages(new() { message });
    }

    public void StartPinging(bool startImmediately = true)
    {
        Logger.Log("StartPinging");
        if (startImmediately) { Ping(); }
        PingTimer.Start();
    }
    public void StopPinging()
    {
        Logger.Log("StopPinging");
        PingTimer.Stop();
    }
}
