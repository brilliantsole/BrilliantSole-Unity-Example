using System.Net.Sockets;
using System.Threading;

public partial class BS_UdpClient
{
    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }

        DidSetRemoteReceivePort = false;

        Logger.Log($"connecting to {ServerIp}:{SendPort}...");
        UdpClient = new(ReceivePort);
        UdpClient.Connect(ServerIp, SendPort);

        IsRunning = true;

        receiveThread ??= new(new ThreadStart(ListenForMessages)) { IsBackground = true };
        receiveThread.Start();

        StartPinging();
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }

        IsRunning = false;

        Reset();
        StopPinging();
        StopWaitingForPong();

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
