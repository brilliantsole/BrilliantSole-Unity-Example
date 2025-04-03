using System;
using System.Collections.Generic;

public static class BS_DeviceManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DeviceManager");

    private static readonly HashSet<BS_Device> availableDevices = new();
    public static IReadOnlyCollection<BS_Device> AvailableDevices => availableDevices;
    public static event Action<IReadOnlyCollection<BS_Device>> OnAvailableDevices;
    public static event Action<BS_Device> OnAvailableDevice;
    public static event Action<BS_Device> OnUnavailableDevice;

    private static readonly HashSet<BS_Device> connectedDevices = new();
    public static IReadOnlyCollection<BS_Device> ConnectedDevices => connectedDevices;
    public static event Action<IReadOnlyCollection<BS_Device>> OnConnectedDevices;
    public static event Action<BS_Device> OnDeviceConnected;
    public static event Action<BS_Device> OnDeviceDisconnected;
    public static event Action<BS_Device, bool> OnDeviceIsConnected;


    public static void OnDeviceCreated(BS_Device device)
    {
        Logger.Log($"adding device \"{device.Name}\"");
        device.OnIsConnected += _OnIsDeviceConnected;
    }

    public static void _OnIsDeviceConnected(BS_Device device, bool isConnected)
    {
        Logger.Log($"device \"{device.Name}\" isConnected? {isConnected}");
        OnDeviceIsConnected?.Invoke(device, isConnected);

        var UpdatedConnectedDevices = false;
        var UpdatedAvailableDevices = false;

        if (isConnected)
        {
            if (!connectedDevices.Contains(device))
            {
                Logger.Log($"device \"{device.Name}\" added to connectedDevices");
                connectedDevices.Add(device);
                UpdatedConnectedDevices = true;
            }

            if (!availableDevices.Contains(device))
            {
                Logger.Log($"device \"{device.Name}\" added to availableDevices");
                availableDevices.Add(device);
                OnAvailableDevice?.Invoke(device);
                UpdatedAvailableDevices = true;
            }

            OnDeviceConnected?.Invoke(device);
            BS_DevicePair.Insoles.AddDevice(device);
            BS_DevicePair.Gloves.AddDevice(device);
        }
        else
        {
            if (connectedDevices.Contains(device))
            {
                Logger.Log($"device \"{device.Name}\" removed from connectedDevices");
                connectedDevices.Remove(device);
                UpdatedConnectedDevices = true;
            }
            if (availableDevices.Contains(device) && !device.IsAvailable)
            {
                Logger.Log($"device \"{device.Name}\" removed from availableDevices");
                availableDevices.Remove(device);
                OnUnavailableDevice?.Invoke(device);
                UpdatedAvailableDevices = true;
            }
            OnDeviceDisconnected?.Invoke(device);
        }

        if (UpdatedConnectedDevices)
        {
            OnConnectedDevices?.Invoke(ConnectedDevices);
        }
        if (UpdatedAvailableDevices)
        {
            OnAvailableDevices?.Invoke(AvailableDevices);
        }
    }
}
