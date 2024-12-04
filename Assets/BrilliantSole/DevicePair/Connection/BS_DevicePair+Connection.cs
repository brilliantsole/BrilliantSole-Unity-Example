using System;
using UnityEngine;

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
    private void CheckIsFullyConnected()
    {
        bool newIsFullyConnected;
        if (!HasAllDevices)
        {
            newIsFullyConnected = true;
            foreach (var pair in Devices)
            {
                newIsFullyConnected = newIsFullyConnected && pair.Value.IsConnected;
                if (!newIsFullyConnected) { break; }
            }
        }
        else
        {
            newIsFullyConnected = false;
        }

        Logger.Log($"newIsFullyConnected: {newIsFullyConnected}");
        IsFullyConnected = newIsFullyConnected;
    }

    public Action<BS_DevicePair, BS_InsoleSide, BS_Device, BS_ConnectionStatus> OnDeviceConnectionStatus;
    private void onDeviceConnectionStatus(BS_Device device, BS_ConnectionStatus connectionStatus)
    {
        OnDeviceConnectionStatus?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, connectionStatus);
    }

    public Action<BS_DevicePair, BS_InsoleSide, BS_Device, bool> OnDeviceIsConnected;
    private void onDeviceIsConnected(BS_Device device, bool isConnected)
    {
        OnDeviceIsConnected?.Invoke(this, (BS_InsoleSide)device.InsoleSide, device, isConnected);
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
