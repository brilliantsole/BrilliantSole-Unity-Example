using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public partial class BS_UdpClient : BS_BaseClient<BS_UdpClient>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClient", BS_Logger.LogLevel.Log);

    private bool IsRunning = false;

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

    private UdpClient UdpClient;

    private Thread receiveThread;
    private void ListenForMessages()
    {
        while (IsRunning)
        {
            try
            {
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, ListeningPort);
                byte[] receivedData = UdpClient.Receive(ref remoteEndPoint);
                if (receivedData.Length > 0)
                {
                    Logger.Log($"received {receivedData.Length} bytes");
                    // FILL
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Error receiving message: {e.Message}");
            }
        }
    }

    protected override void SendData(byte[] data)
    {
        base.SendData(data);
        UdpClient.Send(data, data.Length);
    }
}