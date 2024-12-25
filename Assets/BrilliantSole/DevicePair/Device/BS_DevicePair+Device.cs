using System.Collections.Generic;
using System.Linq;

public partial class BS_DevicePair
{
    private readonly Dictionary<BS_InsoleSide, BS_Device> devices = new();
    public IReadOnlyDictionary<BS_InsoleSide, BS_Device> Devices => devices;

    public BS_Device Left => Devices.GetValueOrDefault(BS_InsoleSide.Left, null);
    public BS_Device Right => Devices.GetValueOrDefault(BS_InsoleSide.Right, null);

    public bool HasAllDevices => Devices.Count == 2;
    private int ConnectedDevicesCount => Devices.Values.Select(device => device.IsConnected).Count();

    public void AddDevice(BS_Device device)
    {
        if (!device.IsInsole)
        {
            Logger.Log($"device is not insole - skipping");
            return;
        }

        var insoleSide = (BS_InsoleSide)device.InsoleSide;

        if (devices.ContainsKey(insoleSide))
        {
            if (devices[insoleSide] == device)
            {
                Logger.Log($"already has device");
                return;
            }
            else
            {
                RemoveDevice(insoleSide);
            }
        }

        Logger.Log($"adding {insoleSide} device \"{device.Name}\"");

        devices.Add(insoleSide, device);
        AddDeviceListeners(device);
        CheckIsFullyConnected();
    }

    public void RemoveDevice(BS_Device device)
    {
        if (!device.IsInsole)
        {
            Logger.Log($"device is not insole");
            return;
        }
        var insoleSide = (BS_InsoleSide)device.InsoleSide;
        if (devices.ContainsKey(insoleSide) && devices[insoleSide] == device)
        {
            RemoveDevice(insoleSide);
        }
        else
        {
            Logger.Log($"device not found");
        }
    }
    public void RemoveDevice(BS_InsoleSide insoleSide)
    {
        if (devices.ContainsKey(insoleSide))
        {
            var device = devices[insoleSide];
            RemoveDeviceListeners(device);
            devices.Remove(insoleSide);
            CheckIsFullyConnected();
        }
        else
        {
            Logger.Log($"no device found for {insoleSide} side");
        }
    }
}
