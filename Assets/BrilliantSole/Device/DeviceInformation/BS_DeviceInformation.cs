using System;
using System.Text;

public class BS_DeviceInformation
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DeviceInformation");

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

    public void UpdateValue(BS_DeviceInformationType deviceInformationType, byte[] data)
    {
        string value = Encoding.UTF8.GetString(data);
        Logger.Log($"Received \"{value}\" for {deviceInformationType}");

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
        bool newHasAllInformation = false;
        if (ManufacturerName == null)
        {
            Logger.Log("ManufacturerName is null");
        }
        else if (ModelNumber == null)
        {
            Logger.Log("ModelNumber is null");
        }
        else if (SerialNumber == null)
        {
            Logger.Log("SerialNumber is null");
        }
        else if (HardwareRevision == null)
        {
            Logger.Log("HardwareRevision is null");
        }
        else if (FirmwareRevision == null)
        {
            Logger.Log("FirmwareRevision is null");
        }
        else if (SoftwareRevision == null)
        {
            Logger.Log("SoftwareRevision is null");
        }
        else
        {
            Logger.Log("Got all Device Information");
            newHasAllInformation = true;
        }

        Logger.Log($"newHasAllInformation: {newHasAllInformation}");

        if (newHasAllInformation == HasAllInformation) { return; }

        HasAllInformation = newHasAllInformation;
        Logger.Log($"HasAllInformation updated to {HasAllInformation}");
    }
}
