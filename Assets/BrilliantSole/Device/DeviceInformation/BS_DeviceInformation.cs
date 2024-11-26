using System;
using System.Text;

[Serializable]
public struct BS_DeviceInformation
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DeviceInformation", BS_Logger.LogLevel.Log);

    public string ManufacturerName { get; private set; }
    public string ModelNumber { get; private set; }
    public string SerialNumber { get; private set; }
    public string HardwareRevision { get; private set; }
    public string FirmwareRevision { get; private set; }
    public string SoftwareRevision { get; private set; }

    public void Clear()
    {
        ManufacturerName = null;
        ModelNumber = null;
        SerialNumber = null;
        HardwareRevision = null;
        FirmwareRevision = null;
        SoftwareRevision = null;
        HasAllInformation = false;
    }

    public void UpdateValue(BS_DeviceInformationType deviceInformationType, byte[] bytes)
    {
        string value = Encoding.UTF8.GetString(bytes);
        switch (deviceInformationType)
        {
            case BS_DeviceInformationType.ManufacturerName:
                ManufacturerName = value;
                break;
            case BS_DeviceInformationType.ModelNumber:
                ModelNumber = value;
                break;
            case BS_DeviceInformationType.SerialNumber:
                SerialNumber = value;
                break;
            case BS_DeviceInformationType.HardwareRevision:
                HardwareRevision = value;
                break;
            case BS_DeviceInformationType.FirmwareRevision:
                FirmwareRevision = value;
                break;
            case BS_DeviceInformationType.SoftwareRevision:
                SoftwareRevision = value;
                break;
            default:
                throw new ArgumentException($"Invalid deviceInformationType {deviceInformationType}");
        }
        UpdateDidGetAllInformation();
    }

    public bool HasAllInformation { get; private set; }
    private void UpdateDidGetAllInformation()
    {
        bool newHasAllInformation = true;
        newHasAllInformation &= ManufacturerName != null;
        newHasAllInformation &= ModelNumber != null;
        newHasAllInformation &= SerialNumber != null;
        newHasAllInformation &= HardwareRevision != null;
        newHasAllInformation &= FirmwareRevision != null;
        newHasAllInformation &= SoftwareRevision != null;

        Logger.Log($"newHasAllInformation: {newHasAllInformation}");

        if (newHasAllInformation == HasAllInformation) { return; }

        HasAllInformation = newHasAllInformation;
        Logger.Log($"HasAllInformation updated to {HasAllInformation}");
    }
}
