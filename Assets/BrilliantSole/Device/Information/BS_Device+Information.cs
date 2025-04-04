using System;
using static BS_DeviceType;
using static BS_Side;

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

        InformationManager.OnName += (name) => OnName?.Invoke(this, name);
        InformationManager.OnId += (id) => OnId?.Invoke(this, id);
        InformationManager.OnCurrentTime += (currentTime) => OnCurrentTime?.Invoke(this, currentTime);
        InformationManager.OnDeviceType += (deviceType) => OnDeviceType?.Invoke(this, deviceType);
        InformationManager.OnMtu += (mtu) => OnMtu?.Invoke(this, mtu);
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
    public bool IsGlove => DeviceType switch
    {
        LeftGlove => true,
        RightGlove => true,
        _ => false
    };
    public BS_Side? Side => DeviceType switch
    {
        LeftInsole => Left,
        LeftGlove => Left,
        RightInsole => Right,
        RightGlove => Right,
        _ => null
    };
}
