using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public static class BS_BleUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleUtils");

    static public string GenerateUuid(string value)
    {
        return string.Format("ea6d{0}-a725-4f9b-893d-c3913e33b39f", value).ToUpper();
    }
    static public string GenerateGenericUuid(string value, bool shorten = true)
    {
        if (shorten) { return value.ToUpper(); }
        return string.Format("0000{0}-0000-1000-8000-00805f9b34fb", value).ToUpper();
    }
    static public bool AreUuidsEqual(string uuid1, string uuid2)
    {
        Logger.Log($"{uuid1} == {uuid2}?");

        if (iOS_UuidMap.ContainsKey(uuid1.ToUpper())) { uuid1 = iOS_UuidMap[uuid1.ToUpper()]; }
        if (uuid1.Length == 4) { uuid1 = GenerateGenericUuid(uuid1, false); }

        if (iOS_UuidMap.ContainsKey(uuid2.ToUpper())) { uuid2 = iOS_UuidMap[uuid2.ToUpper()]; }
        if (uuid2.Length == 4) { uuid2 = GenerateGenericUuid(uuid2, false); }

        return uuid1.ToUpper().Equals(uuid2.ToUpper());
    }

    static public readonly string MainServiceUuid = GenerateUuid("0000");
    static public readonly string RxCharacteristicUuid = GenerateUuid("1000");
    static public readonly string TxCharacteristicUuid = GenerateUuid("1001");

    static public readonly string[] ScanServiceUuids;

    static public readonly string BatteryServiceUuid = GenerateGenericUuid("180f");
    static public readonly string BatteryLevelCharacteristicUuid = GenerateGenericUuid("2a19");

    static public readonly string DeviceInformationServiceUuid = GenerateGenericUuid("180a");
    static public readonly string ModelNumberStringCharacteristicUuid = GenerateGenericUuid("2a24");
    static public readonly string SerialNumberStringCharacteristicUuid = GenerateGenericUuid("2a25");
    static public readonly string FirmwareRevisionCharacteristicUuid = GenerateGenericUuid("2a26");
    static public readonly string HardwareRevisionStringCharacteristicUuid = GenerateGenericUuid("2a27");
    static public readonly string SoftwareRevisionCharacteristicUuid = GenerateGenericUuid("2a28");
    static public readonly string ManufacturerNameStringCharacteristicUuid = GenerateGenericUuid("2a29");

    static public readonly string[] AllCharacteristicUuids;
    static public readonly string[] ReadableCharacteristicUuids;
    static public readonly string[] NotifiableCharacteristicUuids;

    public static readonly Dictionary<string, string[]> ServiceUuids = new()
    {
        { MainServiceUuid, new[] { TxCharacteristicUuid, RxCharacteristicUuid } },
        { BatteryServiceUuid, new[] { BatteryLevelCharacteristicUuid } },
        { DeviceInformationServiceUuid, new[] { ManufacturerNameStringCharacteristicUuid,
            ModelNumberStringCharacteristicUuid,
            SerialNumberStringCharacteristicUuid,
            HardwareRevisionStringCharacteristicUuid,
            FirmwareRevisionCharacteristicUuid,
            SoftwareRevisionCharacteristicUuid } }
    };

    static public readonly string[] AllServiceUuids;

    public static readonly Dictionary<string, string> iOS_UuidMap = new()
    {
        {"BATTERY", BatteryServiceUuid},
        {"BATTERY LEVEL", BatteryLevelCharacteristicUuid},

        {"DEVICE INFORMATION", DeviceInformationServiceUuid},
        {"MODEL NUMBER STRING", ModelNumberStringCharacteristicUuid},
        {"SERIAL NUMBER STRING", SerialNumberStringCharacteristicUuid},
        {"FIRMWARE REVISION STRING", FirmwareRevisionCharacteristicUuid},
        {"HARDWARE REVISION STRING", HardwareRevisionStringCharacteristicUuid},
        {"SOFTWARE REVISION STRING", SoftwareRevisionCharacteristicUuid},
        {"MANUFACTURER NAME STRING", ManufacturerNameStringCharacteristicUuid},

    };

    static public string? GetServiceUuid(string characteristicUuid)
    {
        return ServiceUuids.FirstOrDefault(kvp => kvp.Value.Contains(characteristicUuid)).Key;
    }

    static BS_BleUtils()
    {
        ScanServiceUuids = new[] { MainServiceUuid };

        AllServiceUuids = ServiceUuids.Keys.ToArray();
        AllCharacteristicUuids = ServiceUuids.SelectMany(kvp => kvp.Value).ToArray();

        ReadableCharacteristicUuids = new[] {
            BatteryLevelCharacteristicUuid,
            ManufacturerNameStringCharacteristicUuid,
            ModelNumberStringCharacteristicUuid,
            SerialNumberStringCharacteristicUuid,
            HardwareRevisionStringCharacteristicUuid,
            FirmwareRevisionCharacteristicUuid,
            SoftwareRevisionCharacteristicUuid
        };

        NotifiableCharacteristicUuids = new[] {
            BatteryLevelCharacteristicUuid,
            RxCharacteristicUuid,
        };
    }
}