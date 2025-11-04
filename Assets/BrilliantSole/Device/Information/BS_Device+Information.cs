using System;
using static BS_DeviceType;
using static BS_Side;

public partial class BS_Device
{
    private readonly BS_InformationManager InformationManager = new();

    public delegate void OnNameDelegate(BS_Device device, string name);
    public delegate void OnIdDelegate(BS_Device device, string id);
    public delegate void OnCurrentTimeDelegate(BS_Device device, ulong currentTime);
    public delegate void OnDeviceTypeDelegate(BS_Device device, BS_DeviceType deviceType);
    public delegate void OnMtuDelegate(BS_Device device, ushort mtu);

    public event OnNameDelegate OnName;
    public event OnIdDelegate OnId;
    public event OnCurrentTimeDelegate OnCurrentTime;
    public event OnDeviceTypeDelegate OnDeviceType;
    public event OnMtuDelegate OnMtu;


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
