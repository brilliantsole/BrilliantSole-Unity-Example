using System;
using UnityEngine;
using static BS_ConnectionStatus;

public partial class BS_DevicePair
{
    public event Action<BS_DevicePair, bool> OnIsFullyConnected;

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

    public event Action<BS_DevicePair, bool> OnIsHalfConnected;

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

    public Action<BS_DevicePair, BS_Side, BS_Device, BS_ConnectionStatus> OnDeviceConnectionStatus;
    public Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceConnected;
    public Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceNotConnected;
    public Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceConnecting;
    public Action<BS_DevicePair, BS_Side, BS_Device> OnDeviceDisconnecting;
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

    public Action<BS_DevicePair, BS_Side, BS_Device, bool> OnDeviceIsConnected;
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
