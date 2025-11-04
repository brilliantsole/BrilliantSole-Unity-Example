using System;
using UnityEngine;
using static BS_ConnectionStatus;

public partial class BS_DevicePair
{
    public delegate void OnIsFullyConnectedDelegate(BS_DevicePair devicePair, bool isFullyConnected);
    public delegate void OnIsHalfConnectedDelegate(BS_DevicePair devicePair, bool isHalfConnected);

    public event OnIsFullyConnectedDelegate OnIsFullyConnected;
    public event OnIsHalfConnectedDelegate OnIsHalfConnected;


    [SerializeField]
    private bool isFullyConnected = false;
    public bool IsFullyConnected
    {
        get => isFullyConnected;
        private set
        {
            if (value == isFullyConnected) { return; }
            Logger.Log($"updating IsFullyConnected to {value}");
            isFullyConnected = value;
            OnIsFullyConnected?.Invoke(this, IsFullyConnected);
        }
    }


    [SerializeField]
    private bool isHalfConnected = false;
    public bool IsHalfConnected
    {
        get => isHalfConnected;
        private set
        {
            if (value == isHalfConnected) { return; }
            Logger.Log($"updating IsHalfConnected to {value}");
            isHalfConnected = value;
            OnIsHalfConnected?.Invoke(this, IsHalfConnected);
        }
    }

    private void CheckIsHalfConnected()
    {
        var newIsHalfConnected = ConnectedDevicesCount == 1;
        Logger.Log($"newIsHalfConnected: {newIsHalfConnected}");
        IsHalfConnected = newIsHalfConnected;
    }
    private void CheckIsFullyConnected()
    {
        var newIsFullyConnected = ConnectedDevicesCount == 2;
        Logger.Log($"newIsFullyConnected: {newIsFullyConnected}");
        IsFullyConnected = newIsFullyConnected;

        CheckIsHalfConnected();
    }

    public delegate void OnDeviceConnectionStatusDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        BS_ConnectionStatus connectionStatus
    );
    public delegate void OnDeviceIsConnectedDelegate(
        BS_DevicePair devicePair,
        BS_Side side,
        BS_Device device,
        bool isDeviceConnected
    );

    public event OnDeviceConnectionStatusDelegate OnDeviceConnectionStatus;
    public event OnDeviceDelegate OnDeviceConnected;
    public event OnDeviceDelegate OnDeviceNotConnected;
    public event OnDeviceDelegate OnDeviceConnecting;
    public event OnDeviceDelegate OnDeviceDisconnecting;
    public event OnDeviceIsConnectedDelegate OnDeviceIsConnected;

    private void onDeviceConnectionStatus(BS_Device device, BS_ConnectionStatus connectionStatus)
    {
        OnDeviceConnectionStatus?.Invoke(this, (BS_Side)device.Side, device, connectionStatus);
        switch (connectionStatus)
        {
            case NotConnected:
                OnDeviceNotConnected?.Invoke(this, (BS_Side)device.Side, device);
                break;
            case Connecting:
                OnDeviceConnecting?.Invoke(this, (BS_Side)device.Side, device);
                break;
            case Connected:
                OnDeviceConnected?.Invoke(this, (BS_Side)device.Side, device);
                break;
            case Disconnecting:
                OnDeviceDisconnecting?.Invoke(this, (BS_Side)device.Side, device);
                break;
        }
    }

    private void onDeviceIsConnected(BS_Device device, bool isConnected)
    {
        OnDeviceIsConnected?.Invoke(this, (BS_Side)device.Side, device, isConnected);
        CheckIsFullyConnected();
    }

    private void AddDeviceConnectionListeners(BS_Device device)
    {
        device.OnConnectionStatus += onDeviceConnectionStatus;
        device.OnIsConnected += onDeviceIsConnected;
    }
    private void RemoveDeviceConnectionListeners(BS_Device device)
    {
        device.OnConnectionStatus -= onDeviceConnectionStatus;
        device.OnIsConnected -= onDeviceIsConnected;
    }
}
