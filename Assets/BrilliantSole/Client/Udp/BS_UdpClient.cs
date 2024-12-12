using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public partial class BS_UdpClient : BS_BaseClient<BS_UdpClient>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClient", BS_Logger.LogLevel.Log);

    protected override void Connect(ref bool Continue)
    {
        base.Connect(ref Continue);
        if (!Continue) { return; }

        Logger.Log($"listening to {ServerIp}:{ListeningPort}...");
        UdpClient ??= new();
        UdpClient.Connect(ServerIp, ListeningPort);

        runReceiveThread = true;
        receiveThread ??= new(new ThreadStart(ListenForMessages)) { IsBackground = true };
        receiveThread.Start();

        PingTimer.Start();
    }
    protected override void Disconnect(ref bool Continue)
    {
        base.Disconnect(ref Continue);
        if (!Continue) { return; }

        PingTimer.Stop();

        runReceiveThread = false;
        if (receiveThread?.IsAlive == true) { receiveThread.Join(); }

        UdpClient.Close();
    }

    private UdpClient UdpClient;

    private Thread receiveThread;
    private bool runReceiveThread = false;
    private void ListenForMessages()
    {
        while (runReceiveThread)
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