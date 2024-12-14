using System.Timers;

public partial class BS_UdpClient
{
    private static readonly double PongInterval = 3000;
    private Timer _pongTimer;
    private Timer PongTimer
    {
        get
        {
            if (_pongTimer == null)
            {
                _pongTimer = new(PongInterval) { AutoReset = false };
                _pongTimer.Elapsed += (sender, e) => PongTimeout();
            }
            return _pongTimer;
        }
    }

    static private readonly BS_UdpMessage PongMessage = new(BS_UdpMessageType.Pong);
    private void Pong()
    {
        Logger.Log("Ponging server...");
        SendUdpMessages(new() { PongMessage });
    }

    private void WaitForPong()
    {
        Logger.Log("WaitForPong");
        PongTimer.Start();
    }

    private void StopWaitingForPong()
    {
        Logger.Log("StopWaitingForPong");
        PongTimer.Stop();
    }

    private void PongTimeout()
    {
        Logger.Log($"PongTimeout");
        DidPongTimeout = true;
    }

    private bool DidPongTimeout = false;
    private void CheckPongTimeout()
    {
        if (DidPongTimeout)
        {
            Disconnect();
            DidPongTimeout = false;
        }
    }
}
