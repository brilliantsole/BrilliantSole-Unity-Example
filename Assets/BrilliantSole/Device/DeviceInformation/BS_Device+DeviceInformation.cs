public partial class BS_Device
{
    public readonly BS_DeviceInformation DeviceInformation = new();

    public bool IsUkaton => DeviceInformation.ModelNumber?.Contains("Ukaton") ?? false;
}
