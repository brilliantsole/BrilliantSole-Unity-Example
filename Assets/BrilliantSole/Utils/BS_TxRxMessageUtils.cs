using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public static class BS_TxRxMessageUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TxRxMessageUtils");

    static public readonly ReadOnlyCollection<string> EnumStrings;
    static public readonly ReadOnlyDictionary<string, byte> EnumStringMap;
    private static readonly byte maxTxRxMessageType;
    static BS_TxRxMessageUtils()
    {
        Logger.Log("static constructor");

        List<string> _enumStrings = new();
        Dictionary<string, byte> _enumStringMap = new();

        byte offset = 0;
        BS_BatteryManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_InformationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorConfigurationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_SensorDataManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_VibrationManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_FileTransferManager.InitTxRxEnum(ref offset, _enumStrings);
        BS_TfliteManager.InitTxRxEnum(ref offset, _enumStrings);
        // TODO
        // BS_WifiManager.InitTxRxEnum(ref offset, _enumStrings);
        // BS_CameraManager.InitTxRxEnum(ref offset, _enumStrings);
        // BS_MicrophoneManager.InitTxRxEnum(ref offset, _enumStrings);
        // BS_DisplayManager.InitTxRxEnum(ref offset, _enumStrings);
        maxTxRxMessageType = offset;

        EnumStrings = new(_enumStrings);

        for (byte i = 0; i < _enumStrings.Count; i++) { _enumStringMap[_enumStrings[i]] = i; }
        EnumStringMap = new(_enumStringMap);

        RequiredTxRxMessageTypes = SetupRequiredTxRxMessageTypes();
        RequiredTxRxMessages = SetupRequiredTxRxMessages();
    }

    static public readonly byte[] RequiredTxRxMessageTypes;
    private static byte[] SetupRequiredTxRxMessageTypes()
    {
        List<byte> requiredTxMessageTypes = new();
        requiredTxMessageTypes.AddRange(BS_BatteryManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_InformationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorConfigurationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_SensorDataManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_VibrationManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_TfliteManager.RequiredTxRxMessageTypes);
        requiredTxMessageTypes.AddRange(BS_FileTransferManager.RequiredTxRxMessageTypes);
        return requiredTxMessageTypes.ToArray();
    }

    static public readonly BS_TxMessage[] RequiredTxRxMessages;
    private static BS_TxMessage[] SetupRequiredTxRxMessages()
    {
        List<BS_TxMessage> requiredTxMessages = new();
        foreach (var requiredTxMessageType in RequiredTxRxMessageTypes)
        {
            requiredTxMessages.Add(new(requiredTxMessageType));
        }
        return requiredTxMessages.ToArray();
    }
}
