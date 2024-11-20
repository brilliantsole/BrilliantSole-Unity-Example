using System;

public static class BS_BleUtils
{
    static private string GenerateUuid(string value)
    {
        return string.Format("ea6da725-{0}-4f9b-893d-c3913e33b39f", value);
    }
    static private string GenerateGenericUuid(string value)
    {
        return string.Format("0000{0}-0000-1000-8000-00805f9b34fb", value);
    }

    static bool AreUuidsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
        {
            uuid1 = GenerateUuid(uuid1);
        }
        if (uuid2.Length == 4)
        {
            uuid2 = GenerateUuid(uuid2);
        }

        return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
    }

    static public readonly string MainServiceUuid = GenerateUuid("0000");
    static public readonly string RxCharacteristicUuid = GenerateUuid("1000");
    static public readonly string TxCharacteristicUuid = GenerateUuid("1001");

    static public readonly string[] ServiceUuids;

    static public readonly string BatteryServiceUuid = GenerateGenericUuid("180f");
    static public readonly string BatteryLevelCharacteristicUuid = GenerateGenericUuid("2a19");

    static public readonly string DeviceInformationUuid = GenerateGenericUuid("180a");
    static public readonly string ManufacturerNameStringCharacteristicUuid = GenerateGenericUuid("2a29");
    static public readonly string ModelNumberStringCharacteristicUuid = GenerateGenericUuid("2a24");
    static public readonly string SerialNumberStringCharacteristicUuid = GenerateGenericUuid("2a25");
    static public readonly string HardwareRevisionStringCharacteristicUuid = GenerateGenericUuid("2a27");
    static public readonly string FirmwareRevisionCharacteristicUuid = GenerateGenericUuid("2a26");
    static public readonly string SoftwareRevisionCharacteristicUuid = GenerateGenericUuid("2a28");

    static BS_BleUtils()
    {
        ServiceUuids = new[] { MainServiceUuid };
    }
}