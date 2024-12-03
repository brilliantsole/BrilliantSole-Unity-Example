using System.Collections.Generic;

public partial class BS_DevicePair
{
    private readonly Dictionary<BS_InsoleSide, BS_Device> insoles = new();
    public IReadOnlyDictionary<BS_InsoleSide, BS_Device> Insoles => insoles;

    public BS_Device Left => Insoles.GetValueOrDefault(BS_InsoleSide.Left, null);
    public BS_Device Right => Insoles.GetValueOrDefault(BS_InsoleSide.Right, null);

    public bool HasAllDevices => Insoles.Count == 2;

    public void AddDevice(BS_Device device)
    {
        if (!device.IsInsole)
        {
            Logger.Log($"device is not insole - skipping");
            return;
        }

        var insoleSide = (BS_InsoleSide)device.InsoleSide;

        if (insoles.ContainsKey(insoleSide))
        {
            if (insoles[insoleSide] == device)
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

        insoles.Add(insoleSide, device);
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
        if (insoles.ContainsKey(insoleSide) && insoles[insoleSide] == device)
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
        if (insoles.ContainsKey(insoleSide))
        {
            var device = insoles[insoleSide];
            RemoveDeviceListeners(device);
            insoles.Remove(insoleSide);
            CheckIsFullyConnected();
        }
        else
        {
            Logger.Log($"no device found for {insoleSide} side");
        }
    }
}
