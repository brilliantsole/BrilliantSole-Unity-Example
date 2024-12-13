using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public partial class BS_UdpClient
{
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

    protected override void SendMessageData(List<byte> data, bool sendImmediately)
    {
        base.SendMessageData(data, sendImmediately);
        //SendUdpMessages();
    }
}
