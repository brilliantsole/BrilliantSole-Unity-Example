using System;
using System.Linq;
using UnityEngine;
using static BS_ConnectionStatus;

#nullable enable

public partial class BS_Device
{
    private BS_BaseConnectionManager? _connectionManager;
    public BS_BaseConnectionManager? ConnectionManager
    {
        get => _connectionManager;
        set
        {
            if (_connectionManager == value) { return; }

            if (_connectionManager != null)
            {
                _connectionManager.OnStatus = null;
                _connectionManager.OnBatteryLevel = null;
                _connectionManager.OnRxMessage = null;
                _connectionManager.OnRxMessages = null;
                _connectionManager.OnDeviceInformationValue = null;
                _connectionManager.OnSendTxData = null;
            }
            if (value != null)
            {
                value.OnStatus = OnConnectionManagerStatus;
                value.OnBatteryLevel = OnBatteryLevelUpdate;
                value.OnRxMessage = OnRxMessage;
                value.OnRxMessages = OnRxMessages;
                value.OnDeviceInformationValue = OnDeviceInformationValue;
                value.OnSendTxData = OnSendTxData;
                if (value.Name != null) { InformationManager.InitName(Name); }
                if (value.DeviceType != null) { InformationManager.InitDeviceType(DeviceType); }
            }

            _connectionManager = value;
        }
    }

    private void OnConnectionManagerStatus(BS_BaseConnectionManager connectionManager, BS_ConnectionStatus _ConnectionManagerStatus)
    {
        Logger.Log($"ConnectionManagerStatus updated to {_ConnectionManagerStatus}");
        switch (_ConnectionManagerStatus)
        {
            case Connected:
                SendTxMessages(BS_TxRxMessageUtils.RequiredTxRxMessages);
                break;
            case NotConnected:
                Reset();
                break;
        }

        if (_ConnectionManagerStatus != Connected)
        {
            ConnectionStatus = _ConnectionManagerStatus;
        }
    }

    public BS_ConnectionType? ConnectionType => ConnectionManager?.Type;
    private BS_ConnectionStatus ConnectionManagerStatus => ConnectionManager?.Status ?? NotConnected;

    public bool IsAvailable => ConnectionManager?.IsAvailable ?? false;

    public event Action<BS_Device, BS_ConnectionStatus>? OnConnectionStatus;
    public event Action<BS_Device, bool>? OnIsConnected;

    [SerializeField]
    private BS_ConnectionStatus _connectionStatus;
    public BS_ConnectionStatus ConnectionStatus
    {
        get => _connectionStatus;
        private set
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

    public void _SetConnectionStatus(BS_ConnectionStatus connectionStatus) { ConnectionStatus = connectionStatus; }

    public void Connect() { ConnectionManager?.Connect(); }
    public void Disconnect() { ConnectionManager?.Disconnect(); }
    public void ToggleConnection()
    {
        switch (ConnectionManagerStatus)
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

    private void OnBatteryLevelUpdate(BS_BaseConnectionManager connectionManager, byte batteryLevel)
    {
        Logger.Log($"Received battery level {batteryLevel}%");
        BatteryLevel = batteryLevel;
    }
    private void OnDeviceInformationValue(BS_BaseConnectionManager connectionManager, BS_DeviceInformationType deviceInformationType, byte[] data)
    {
        Logger.Log($"Received {data.Length} data for DeviceInformationType {deviceInformationType}");
        DeviceInformation.UpdateValue(deviceInformationType, data);
    }
}
