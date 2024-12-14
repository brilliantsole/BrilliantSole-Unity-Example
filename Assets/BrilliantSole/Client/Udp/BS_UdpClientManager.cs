using System;
using System.Net;
using UnityEngine.Events;
using static BS_ConnectionStatus;

public class BS_UdpClientManager : BS_SingletonMonoBehavior<BS_UdpClientManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClientManager", BS_Logger.LogLevel.Log);
    private static BS_UdpClient Client => BS_UdpClient.Instance;
    public void Update() { Client.Update(); }

    public string ServerIp => Client.ServerIp;
    public void SetServerIp(string newServerIp)
    {
        if (!IPAddress.TryParse(newServerIp, out _))
        {
            Logger.LogError($"invalid ip address {newServerIp}");
            return;
        }

        Client.ServerIp = newServerIp;
    }

    private int? ParsePortString(string portString)
    {
        if (int.TryParse(portString, out int port) && port >= 0 && port <= 65535)
        {
            return port;
        }
        else
        {
            Logger.LogError($"invalid port {portString}");
            return null;
        }
    }

    public int SendPort => Client.SendPort;
    public void SetSendPort(int newSendPort) { Client.SendPort = newSendPort; }
    public void SetSendPort(string newSendPortString)
    {
        var newSendPort = ParsePortString(newSendPortString);
        if (newSendPort != null) { SetSendPort((int)newSendPort); }
    }

    public int ReceivePort => Client.ReceivePort;
    public void SetReceivePort(int newReceivePort) { Client.ReceivePort = newReceivePort; }
    public void SetReceivePort(string newReceivePortString)
    {
        var newReceivePort = ParsePortString(newReceivePortString);
        if (newReceivePort != null) { SetSendPort((int)newReceivePort); }
    }

    public void ToggleConnection() { Client.ToggleConnection(); }
    public void Connect() { Client.Connect(); }
    public void Disconnect() { Client.Disconnect(); }

    [Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    public BoolUnityEvent OnIsConnected;

    [Serializable]
    public class ConnectionStatusUnityEvent : UnityEvent<BS_ConnectionStatus> { }
    public ConnectionStatusUnityEvent OnConnectionStatus;

    public UnityEvent OnNotConnected;
    public UnityEvent OnConnecting;
    public UnityEvent OnConnected;
    public UnityEvent OnDisconnecting;

    private void OnEnable()
    {
        Client.OnConnectionStatus += onConnectionStatus;
        Client.OnIsConnected += onIsConnected;
        // FILL - Add Listeners
    }

    private void OnDisable()
    {
        Client.OnConnectionStatus -= onConnectionStatus;
        Client.OnIsConnected -= onIsConnected;
        // FILL - Remove Listeners
    }

    private void onConnectionStatus(BS_BaseClient client, BS_ConnectionStatus connectionStatus)
    {
        Logger.Log($"ConnectionStatus: {connectionStatus}");
        OnConnectionStatus?.Invoke(connectionStatus);
        switch (connectionStatus)
        {
            case NotConnected:
                OnNotConnected?.Invoke();
                break;
            case Connecting:
                OnConnecting?.Invoke();
                break;
            case Connected:
                OnConnected?.Invoke();
                break;
            case Disconnecting:
                OnDisconnecting?.Invoke();
                break;
        }
    }
    private void onIsConnected(BS_BaseClient client, bool isConnected) { OnIsConnected?.Invoke(isConnected); }
}
