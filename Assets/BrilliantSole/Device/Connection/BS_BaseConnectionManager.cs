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
                Logger.Log($"Updating ConnectionManager Status to {value}");
                _status = value;
                OnStatus?.Invoke(Status);
            }
        }
    }

    public bool IsConnected => Status == BS_ConnectionStatus.Connected;

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
        Status = BS_ConnectionStatus.Connecting;
        Logger.Log("Connecting...");
    }
    public void Disconnect()
    {
        bool Continue = true;
        Disconnect(ref Continue);
    }
    protected virtual void Disconnect(ref bool Continue)
    {
        if (!IsConnected)
        {
            Logger.Log("Already not connected");
            Continue = false;
            return;
        }
        Status = BS_ConnectionStatus.Disconnecting;
        Logger.Log("Disconnecting...");
    }

    public virtual void Update() { }
}
