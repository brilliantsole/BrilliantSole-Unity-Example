using System;

public static class BS_BleUtils
{
    static private string GenerateUuid(string value)
    {
        return String.Format("ea6da725-{0}-4f9b-893d-c3913e33b39f", value);
    }
    static private string GenerateGenericUuid(string value)
    {
        return String.Format("0000{0}-0000-1000-8000-00805f9b34fb", value);
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

    static private string mainServiceUuid = GenerateUuid("0000");
    static private string rxCharacteristicUuid = GenerateUuid("1000");
    static private string txCharacteristicUuid = GenerateUuid("1001");

    static private string batteryServiceUuid = GenerateGenericUuid("180f");
    static private string batteryLevelCharacteristicUuid = GenerateGenericUuid("2a19");

    static private string deviceInformationUuid = GenerateGenericUuid("180a");
    static private string manufacturerNameStringCharacteristicUuid = GenerateGenericUuid("2a29");
    static private string modelNumberStringCharacteristicUuid = GenerateGenericUuid("2a24");
    static private string serialNumberStringCharacteristicUuid = GenerateGenericUuid("2a25");
    static private string hardwareRevisionStringCharacteristicUuid = GenerateGenericUuid("2a27");
    static private string firmwareRevisionCharacteristicUuid = GenerateGenericUuid("2a26");
    static private string softwareRevisionCharacteristicUuid = GenerateGenericUuid("2a28");
}