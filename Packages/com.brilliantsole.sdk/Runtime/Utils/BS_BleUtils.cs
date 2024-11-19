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

    static readonly string mainServiceUuid = GenerateUuid("0000");
    static readonly string rxCharacteristicUuid = GenerateUuid("1000");
    static readonly string txCharacteristicUuid = GenerateUuid("1001");

    static readonly string batteryServiceUuid = GenerateGenericUuid("180f");
    static readonly string batteryLevelCharacteristicUuid = GenerateGenericUuid("2a19");

    static readonly string deviceInformationUuid = GenerateGenericUuid("180a");
    static readonly string manufacturerNameStringCharacteristicUuid = GenerateGenericUuid("2a29");
    static readonly string modelNumberStringCharacteristicUuid = GenerateGenericUuid("2a24");
    static readonly string serialNumberStringCharacteristicUuid = GenerateGenericUuid("2a25");
    static readonly string hardwareRevisionStringCharacteristicUuid = GenerateGenericUuid("2a27");
    static readonly string firmwareRevisionCharacteristicUuid = GenerateGenericUuid("2a26");
    static readonly string softwareRevisionCharacteristicUuid = GenerateGenericUuid("2a28");
}