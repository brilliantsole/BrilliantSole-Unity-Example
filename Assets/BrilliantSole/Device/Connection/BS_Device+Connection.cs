using System;
using UnityEngine;

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
                _connectionManager.OnSendTxMessage = null;
            }
            if (value != null)
            {
                value.OnStatus = OnConnectionManagerStatus;
                value.OnBatteryLevel = OnBatteryLevelUpdate;
                value.OnRxMessage = OnRxMessage;
                value.OnRxMessages = OnRxMessages;
                value.OnDeviceInformationValue = OnDeviceInformationValue;
                value.OnSendTxMessage = OnSendTxMessage;
            }

            _connectionManager = value;
        }
    }

    private void OnConnectionManagerStatus(BS_BaseConnectionManager connectionManager, BS_ConnectionStatus _ConnectionManagerStatus)
    {
        Logger.Log($"ConnectionManagerStatus updated to {_ConnectionManagerStatus}");
        switch (_ConnectionManagerStatus)
        {
            case BS_ConnectionStatus.Connected:
                // FILL - send Required Tx Messages
                break;
            case BS_ConnectionStatus.NotConnected:
                Reset();
                break;
        }

        if (_ConnectionManagerStatus != BS_ConnectionStatus.Connected)
        {
            ConnectionStatus = _ConnectionManagerStatus;
        }
    }

    public BS_ConnectionType? ConnectionType => ConnectionManager?.Type;
    private BS_ConnectionStatus ConnectionManagerStatus => ConnectionManager?.Status ?? BS_ConnectionStatus.NotConnected;

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
                case BS_ConnectionStatus.Connected:
                case BS_ConnectionStatus.NotConnected:
                    OnIsConnected?.Invoke(this, IsConnected);
                    break;
            }
        }
    }
    public bool IsConnected => ConnectionStatus == BS_ConnectionStatus.Connected;

    public void Connect() { ConnectionManager?.Connect(); }
    public void Disconnect() { ConnectionManager?.Disconnect(); }
    public void ToggleConnection() { if (IsConnected) { Disconnect(); } else { Connect(); } }

    private void OnBatteryLevelUpdate(BS_BaseConnectionManager connectionManager, byte batteryLevel)
    {
        Logger.Log($"Received battery level {batteryLevel}%");
        BatteryLevel = batteryLevel;
    }
    private void OnRxMessage(BS_BaseConnectionManager connectionManager)
    {
        // FILL
    }
    private void OnRxMessages(BS_BaseConnectionManager connectionManager)
    {
        // FILL
    }
    private void OnDeviceInformationValue(BS_BaseConnectionManager connectionManager, BS_DeviceInformationType deviceInformationType, byte[] bytes)
    {
        Logger.Log($"Received {bytes.Length} bytes for DeviceInformationType {deviceInformationType}");
        DeviceInformation.UpdateValue(deviceInformationType, bytes);
    }
    private void OnSendTxMessage(BS_BaseConnectionManager connectionManager)
    {
        // FILL
    }
}
