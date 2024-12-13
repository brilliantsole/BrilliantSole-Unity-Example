using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public partial class BS_UdpClient
{
    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }

        Logger.Log($"listening to {ServerIp}:{ListeningPort}...");
        UdpClient = new(ListeningPort);
        UdpClient.Connect(ServerIp, SendingPort);

        IsRunning = true;

        receiveThread ??= new(new ThreadStart(ListenForMessages)) { IsBackground = true };
        receiveThread.Start();

        Ping();
        PingTimer.Start();
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }

        IsRunning = false;

        PingTimer.Stop();

        UdpClient?.Close();
        UdpClient = null;

        if (receiveThread != null)
        {
            if (receiveThread.IsAlive) { receiveThread.Join(); }
            receiveThread = null;
        }

        ConnectionStatus = BS_ConnectionStatus.NotConnected;
    }
}
