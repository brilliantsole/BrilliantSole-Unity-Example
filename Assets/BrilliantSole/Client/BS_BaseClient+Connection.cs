using System;
using UnityEngine;
using static BS_ConnectionStatus;

public partial class BS_BaseClient
{
    public event Action<BS_BaseClient, BS_ConnectionStatus> OnConnectionStatus;
    public event Action<BS_BaseClient, bool> OnIsConnected;

    [SerializeField]
    private BS_ConnectionStatus _connectionStatus;
    public BS_ConnectionStatus ConnectionStatus
    {
        get => _connectionStatus;
        protected set
        {
            if (_connectionStatus == value) { return; }
            Logger.Log($"Updating Connection Status to {value}");
            _connectionStatus = value;
            OnConnectionStatus?.Invoke(this, ConnectionStatus);

            switch (ConnectionStatus)
            {
                case Connected:
                case NotConnected:
                    OnIsConnected?.Invoke(this, IsConnected);
                    break;
            }
        }
    }
    public bool IsConnected => ConnectionStatus == Connected;

    public void Connect()
    {
        bool Continue = true;
        Connect(ref Continue);
    }
    protected virtual void Connect(ref bool Continue)
    {
        if (IsConnected)
        {
            Logger.Log("Already connected");
            Continue = true;
            return;
        }
        ConnectionStatus = Connecting;
        Logger.Log("Connecting...");
    }
    public void Disconnect()
    {
        bool Continue = true;
        Disconnect(ref Continue);
    }
    protected virtual void Disconnect(ref bool Continue)
    {
        if (ConnectionStatus == NotConnected)
        {
            Logger.Log("Already not connected");
            Continue = false;
            return;
        }
        if (ConnectionStatus == Disconnecting)
        {
            Logger.Log("Already Disconnecting");
            Continue = false;
            return;
        }
        ConnectionStatus = Disconnecting;
        Logger.Log("Disconnecting...");
    }
    public void ToggleConnection()
    {
        switch (ConnectionStatus)
        {
            case Connected:
            case Connecting:
                Disconnect();
                break;
            default:
                Connect();
                break;
        }
    }
}