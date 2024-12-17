using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static BS_ConnectionStatus;

public partial class BS_UdpClient
{
    private Thread receiveThread;
    private readonly Queue<byte[]> messageQueue = new();
    private readonly object queueLock = new();
    private void ListenForMessages()
    {
        while (IsRunning)
        {
            try
            {
                Logger.Log($"listening on {ReceivePort}...");
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var receivedData = UdpClient.Receive(ref remoteEndPoint);
                Logger.Log($"received {receivedData.Length} bytes");
                if (receivedData.Length > 0)
                {
                    lock (queueLock) { messageQueue.Enqueue(receivedData); }
                }
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004) { Logger.LogError($"Socket exception while receiving data from udp client: {e.Message}"); }
            }
            catch (Exception e)
            {
                Logger.LogError($"Error receiving message: {e.Message}");
            }
        }
    }

    private void ParseReceivedMessages()
    {
        lock (queueLock)
        {
            while (messageQueue.Count > 0)
            {
                var messageData = messageQueue.Dequeue();
                OnUdpData(messageData);
            }
        }
    }

    private void OnUdpData(in byte[] data)
    {
        Logger.Log($"parsing {data.Length} bytes...");
        BS_ParseUtils.ParseMessages(data, OnUdpMessage);
    }
    private void OnUdpMessage(byte udpMessageTypeByte, byte[] data)
    {
        if (!Enum.IsDefined(typeof(BS_UdpMessageType), udpMessageTypeByte))
        {
            Logger.LogError($"invalid udpMessageTypeByte {udpMessageTypeByte}");
            return;
        }
        var udpMessageType = (BS_UdpMessageType)udpMessageTypeByte;
        Logger.Log($"udpMessageType: {udpMessageType}");

        StopWaitingForPong();

        switch (udpMessageType)
        {
            case BS_UdpMessageType.Ping:
                Pong();
                break;
            case BS_UdpMessageType.Pong:
                break;
            case BS_UdpMessageType.SetRemoteReceivePort:
                ParseRemoteReceivePort(data);
                break;
            case BS_UdpMessageType.ServerMessage:
                OnData(data);
                break;
            default:
                Logger.LogError($"Uncaught udpMessageType {udpMessageType}");
                break;
        }

        if (IsConnected)
        {
            WaitForPong();
        }
    }

    protected override void SendMessageData(List<byte> data, bool sendImmediately = true)
    {
        base.SendMessageData(data, sendImmediately);
        SendUdpMessages(new() { new(BS_UdpMessageType.ServerMessage, data) }, sendImmediately);
    }

    private readonly List<BS_UdpMessage> PendingUdpMessages = new();
    private readonly List<byte> UdpData = new();
    private bool IsSendingUdpData = false;
    private static readonly ushort MaxUdpMessageSize = 65507;
    private void SendUdpMessages(List<BS_UdpMessage> udpMessages, bool sendImmediately = true)
    {
        Logger.Log($"requesting to send {udpMessages.Count} messages...");
        PendingUdpMessages.AddRange(udpMessages);

        if (!sendImmediately)
        {
            Logger.Log($"not sending data immediately");
            return;
        }
        if (IsSendingUdpData)
        {
            Logger.Log($"already sending data - will wait");
            return;
        }
        SendPendingUdpMessages();
    }
    private void SendPendingUdpMessages()
    {
        if (PendingUdpMessages.Count == 0)
        {
            //Logger.Log("PendingUdpMessages is empty");
            return;
        }
        IsSendingUdpData = true;
        UdpData.Clear();

        var pendingUdpMessageIndex = 0;
        while (pendingUdpMessageIndex < PendingUdpMessages.Count)
        {
            var pendingUdpMessage = PendingUdpMessages[pendingUdpMessageIndex];
            var pendingUdpMessageLength = pendingUdpMessage.Length();
            bool shouldAppendUdpMessage = UdpData.Count + pendingUdpMessageLength <= MaxUdpMessageSize;
            if (shouldAppendUdpMessage)
            {
                Logger.Log($"appending message \"{pendingUdpMessage.Type}\" ({pendingUdpMessageLength} bytes)");
                pendingUdpMessage.AppendTo(UdpData);
                PendingUdpMessages.RemoveAt(pendingUdpMessageIndex);
            }
            else
            {
                Logger.Log($"skipping message \"{pendingUdpMessage.Type}\" ({pendingUdpMessageLength} bytes)");
                pendingUdpMessageIndex++;
            }
        }

        SendUdpData();
    }

    private void SendUdpData()
    {
        var messageBytes = UdpData.ToArray();
        Logger.Log($"sending {messageBytes.Length} bytes...");
        UdpClient.Send(messageBytes, messageBytes.Length);
        Logger.Log($"sent {messageBytes.Length} bytes");
        IsSendingUdpData = false;
        SendPendingUdpMessages();
    }

    private void ParseRemoteReceivePort(in byte[] data)
    {
        var parsedReceivePort = BS_ByteUtils.ParseNumber<ushort>(data);
        Logger.Log($"parsedReceivePort: {parsedReceivePort}");
        if (parsedReceivePort != ReceivePort)
        {
            Logger.LogError($"invalid receivePort - expected {ReceivePort}, got {parsedReceivePort}");
            return;
        }

        Logger.Log("successfully set ReceivePort");
        DidSetRemoteReceivePort = true;

        ConnectionStatus = Connected;
    }
}
