using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BS_BaseConnectionManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseConnectionManager", BS_Logger.LogLevel.Log);

    public abstract BS_ConnectionType Type { get; }

    public Action<BS_BaseConnectionManager, byte> OnBatteryLevel;
    public Action<BS_BaseConnectionManager, byte, byte[]> OnRxMessage;
    public Action<BS_BaseConnectionManager> OnRxMessages;
    public Action<BS_BaseConnectionManager, BS_DeviceInformationType, byte[]> OnDeviceInformationValue;
    public Action<BS_BaseConnectionManager> OnSendTxData;


    public Action<BS_BaseConnectionManager, BS_ConnectionStatus> OnStatus;

    [SerializeField]
    private BS_ConnectionStatus _status = BS_ConnectionStatus.NotConnected;
    public BS_ConnectionStatus Status
    {
        get => _status;
        protected set
        {
            if (_status == value) { return; }
            Logger.Log($"Updating ConnectionManager Status to {value}");
            _status = value;
            OnStatus?.Invoke(this, Status);
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
        if (Status == BS_ConnectionStatus.NotConnected)
        {
            Logger.Log("Already not connected");
            Continue = false;
            return;
        }
        if (Status == BS_ConnectionStatus.Disconnecting)
        {
            Logger.Log("Already Disconnecting");
            Continue = false;
            return;
        }
        Status = BS_ConnectionStatus.Disconnecting;
        Logger.Log("Disconnecting...");
    }

    public virtual void Update() { }

    protected void ParseRxData(byte[] data)
    {
        Logger.Log($"Parsing {data.Length} data of Rx data...");
        BS_ParseUtils.ParseMessages(data, OnParsedRxMessage);
        OnRxMessages?.Invoke(this);
    }

    private void OnParsedRxMessage(byte messageType, byte[] messageData) { OnRxMessage(this, messageType, messageData); }

    public virtual void SendTxData(List<byte> Data) { Logger.Log($"Sending {Data.Count} bytes..."); }
}
