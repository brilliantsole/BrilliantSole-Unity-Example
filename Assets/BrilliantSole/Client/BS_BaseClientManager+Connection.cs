using System;
using UnityEngine;
using UnityEngine.Events;
using static BS_ConnectionStatus;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    [SerializeField]
    private bool reconnectOnDisconnection = false;
    public bool ReconnectOnDisconnection
    {
        get => reconnectOnDisconnection;
        set
        {
            if (reconnectOnDisconnection != value)
            {
                reconnectOnDisconnection = value;
                Client.ReconnectOnDisconnection = value;
            }
        }
    }

    public void ToggleConnection() { Client.ToggleConnection(); }
    public void Connect() { Client.Connect(); }
    public void Disconnect() { Client.Disconnect(); }

    public BS_ConnectionStatus ConnectionStatus => Client.ConnectionStatus;
    public bool IsConnected => Client.IsConnected;

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
