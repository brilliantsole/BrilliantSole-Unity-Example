using System;
using System.Collections.Generic;

public static class BS_DeviceManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DeviceManager");

    private static readonly HashSet<BS_Device> availableDevices = new();
    public static IReadOnlyCollection<BS_Device> AvailableDevices => availableDevices;
    public static event Action<IReadOnlyCollection<BS_Device>> OnAvailableDevices;
    public static event Action<BS_Device> OnAvailableDevice;

    private static readonly HashSet<BS_Device> connectedDevices = new();
    public static IReadOnlyCollection<BS_Device> ConnectedDevices => connectedDevices;
    public static event Action<IReadOnlyCollection<BS_Device>> OnConnectedDevices;
    public static event Action<BS_Device> OnDeviceConnected;
    public static event Action<BS_Device> OnDeviceDisconnected;
    public static event Action<BS_Device, bool> OnDeviceIsConnected;


    public static void OnDeviceCreated(BS_Device device)
    {
        Logger.Log($"adding device \"{device.Name}\"");
        device.OnIsConnected += OnIsDeviceConnected;
    }

    private static void OnIsDeviceConnected(BS_Device device, bool isConnected)
    {
        Logger.Log($"device \"{device.Name}\" isConnected? {isConnected}");
        if (isConnected)
        {
            connectedDevices.Add(device);
            if (!availableDevices.Contains(device))
            {
                availableDevices.Add(device);
                OnAvailableDevices?.Invoke(AvailableDevices);
                OnAvailableDevice?.Invoke(device);
            }
            OnDeviceConnected?.Invoke(device);
        }
        else
        {
            connectedDevices.Remove(device);
            OnDeviceDisconnected?.Invoke(device);
        }
        OnConnectedDevices?.Invoke(ConnectedDevices);
        OnDeviceIsConnected?.Invoke(device, isConnected);
    }
}
