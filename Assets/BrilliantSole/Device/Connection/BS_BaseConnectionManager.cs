using System;
using UnityEngine;

public abstract class BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseConnectionManager", BS_Logger.LogLevel.Log);

    public abstract BS_ConnectionType Type { get; }

    public delegate void OnStatusDelegate(BS_ConnectionStatus connectionStatus);
    public OnStatusDelegate OnStatus;


    [SerializeField]
    private BS_ConnectionStatus _status;
    public BS_ConnectionStatus Status
    {
        get => _status;
        protected set
        {
            if (_status != value)
            {
                Logger.Log($"Updating Connection Status to {value}");
                _status = value;
                OnStatus?.Invoke(Status);
            }
        }
    }

    public bool IsConnected => Status == BS_ConnectionStatus.Connected;

    public void Connect()
    {
        bool Continue = false;
        Connect(Continue);
    }
    protected virtual void Connect(in bool Continue)
    {
        if (IsConnected)
        {
            Logger.Log("Already connected");
            return;
        }
        Status = BS_ConnectionStatus.Connecting;
        Logger.Log("Connecting...");
    }
    public void Disconnect()
    {
        bool Continue = false;
        Disconnect(Continue);
    }
    protected virtual void Disconnect(in bool Continue)
    {
        if (!IsConnected)
        {
            Logger.Log("Already not connected");
            return;
        }
        Status = BS_ConnectionStatus.Disconnecting;
        Logger.Log("Disconnecting...");
    }
}
