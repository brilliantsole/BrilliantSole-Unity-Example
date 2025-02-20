using System;
using System.Collections.Generic;
using UnityEngine;
using static BS_ConnectionStatus;

public partial class BS_BaseClient
{
    public event Action<BS_BaseClient, BS_ConnectionStatus> OnConnectionStatus;
    public event Action<BS_BaseClient, bool> OnIsConnected;

    public bool ReconnectOnDisconnection = false;
    protected bool DisconnectedUnintentionally = false;

    [SerializeField]
    private BS_ConnectionStatus _connectionStatus;
    public BS_ConnectionStatus ConnectionStatus
    {
        get => _connectionStatus;
        protected set
        {
            if (_connectionStatus == value)
            {
                Logger.Log($"redundant connectionStatus {value}");
                return;
            }
            _connectionStatus = value;
            Logger.Log($"Updated ConnectionStatus to {ConnectionStatus}");
            OnConnectionStatus?.Invoke(this, ConnectionStatus);

            switch (ConnectionStatus)
            {
                case Connected:
                case NotConnected:
                    OnIsConnected?.Invoke(this, IsConnected);
                    if (ConnectionStatus == Connected)
                    {
                        // SendRequiredMessages(false);
                    }
                    else
                    {
                        Reset();
                        if (DisconnectedUnintentionally && ReconnectOnDisconnection)
                        {
                            Logger.Log($"attempting reconnection");
                            Connect();
                            DisconnectedUnintentionally = false;
                        }
                    }
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
            Continue = false;
            return;
        }
        ConnectionStatus = Connecting;
        Logger.Log("Connecting...");
        Continue = true;
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
        Continue = true;
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

    private readonly HashSet<BS_ServerMessageType> receivedMessageTypes = new();
    private void CheckIfFullyConnected()
    {
        if (ConnectionStatus != Connecting)
        {
            return;
        }
        Logger.Log("checking if fully connected...");

        if (!receivedMessageTypes.Contains(BS_ServerMessageType.IsScanningAvailable))
        {
            Logger.Log("didn't receive isScanningAvailable yet - not fully connected");
            return;
        }

        if (IsScanningAvailable)
        {
            if (!receivedMessageTypes.Contains(BS_ServerMessageType.IsScanning))
            {
                Logger.Log("didn't receive isScanning yet - not fully connected");
                return;
            }
        }

        Logger.Log("fully connected");
        ConnectionStatus = Connected;
    }
}
