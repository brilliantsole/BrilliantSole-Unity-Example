using System;
using static BS_DeviceType;
using static BS_InsoleSide;

public partial class BS_Device
{
    private readonly BS_InformationManager InformationManager = new();

    public event Action<BS_Device, string> OnName;
    public event Action<BS_Device, string> OnId;
    public event Action<BS_Device, ulong> OnCurrentTime;
    public event Action<BS_Device, BS_DeviceType> OnDeviceType;
    public event Action<BS_Device, ushort> OnMtu;
    private void SetupInformationManager()
    {
        Managers.Add(InformationManager);

        InformationManager.OnName += (string name) => OnName?.Invoke(this, name);
        InformationManager.OnId += (string id) => OnId?.Invoke(this, id);
        InformationManager.OnCurrentTime += (ulong currentTime) => OnCurrentTime?.Invoke(this, currentTime);
        InformationManager.OnDeviceType += (BS_DeviceType deviceType) => OnDeviceType?.Invoke(this, deviceType);
        InformationManager.OnMtu += (ushort mtu) => OnMtu?.Invoke(this, mtu);
    }

    public string Name => InformationManager.Name;
    public string Id => InformationManager.Id;
    public ulong CurrentTime => InformationManager.CurrentTime;
    public BS_DeviceType DeviceType => InformationManager.DeviceType;
    public ushort Mtu => InformationManager.Mtu;

    public bool IsInsole => DeviceType switch
    {
        LeftInsole => true,
        RightInsole => true,
        _ => false
    };
    public BS_InsoleSide? InsoleSide => DeviceType switch
    {
        LeftInsole => Left,
        RightInsole => Right,
        _ => null
    };
}
