using System.Collections.Generic;
using System.Linq;

public partial class BS_DevicePair
{
    private readonly Dictionary<BS_Side, BS_Device> devices = new();
    public IReadOnlyDictionary<BS_Side, BS_Device> Devices => devices;

    public BS_Device GetDevice(BS_Side side) => Devices.GetValueOrDefault(side, null);
    public BS_Device Left => GetDevice(BS_Side.Left);

    public BS_Device Right => GetDevice(BS_Side.Right);


    public bool HasAllDevices => Devices.Count == 2;
    private int ConnectedDevicesCount => Devices.Values.Select(device => device.IsConnected).Count();

    public void AddDevice(BS_Device device)
    {
        if (Type == BS_DevicePairType.Insoles && !device.IsInsole)
        {
            Logger.Log($"device is not insole - skipping");
            return;
        }
        if (Type == BS_DevicePairType.Gloves && !device.IsGlove)
        {
            Logger.Log($"device is not glove - skipping");
            return;
        }

        var side = (BS_Side)device.Side;

        if (devices.ContainsKey(side))
        {
            if (devices[side] == device)
            {
                Logger.Log($"already has device");
                return;
            }
            else
            {
                RemoveDevice(side);
            }
        }

        Logger.Log($"adding {side} device \"{device.Name}\"");

        devices.Add(side, device);
        AddDeviceListeners(device);
        CheckIsFullyConnected();

        onDeviceConnectionStatus(device, device.ConnectionStatus);
        onDeviceIsConnected(device, device.IsConnected);
    }

    public void RemoveDevice(BS_Device device)
    {
        if (!device.IsInsole)
        {
            Logger.Log($"device is not insole");
            return;
        }
        var side = (BS_Side)device.Side;
        if (devices.ContainsKey(side) && devices[side] == device)
        {
            RemoveDevice(side);
        }
        else
        {
            Logger.Log($"device not found");
        }
    }
    public void RemoveDevice(BS_Side side)
    {
        if (devices.ContainsKey(side))
        {
            var device = devices[side];
            RemoveDeviceListeners(device);
            devices.Remove(side);
            CheckIsFullyConnected();
        }
        else
        {
            Logger.Log($"no device found for {side} side");
        }
    }
}
