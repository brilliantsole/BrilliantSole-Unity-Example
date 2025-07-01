using System.Collections.Generic;

public class BS_VibrationManager : BS_BaseManager<BS_VibrationMessageType>
{
    public static readonly BS_VibrationMessageType[] RequiredMessageTypes = {
        BS_VibrationMessageType.GetVibrationLocations
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_VibrationManager", BS_Logger.LogLevel.Log);

    public override void OnRxMessage(BS_VibrationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case BS_VibrationMessageType.GetVibrationLocations:
                // FILL
                Logger.Log("GetVibrationLocations");
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public override void Reset() { base.Reset(); }

    public void TriggerVibration(List<BS_VibrationConfiguration> vibrationConfigurations)
    {
        Logger.Log("triggering vibration...");
        List<byte> TxData = new();
        for (int i = 0; i < vibrationConfigurations.Count; i++)
        {
            var array = vibrationConfigurations[i].ToArray();
            if (array.Count == 0)
            {
                Logger.Log("empty vibrationConfiguration array - skipping");
                continue;
            }
            TxData.AddRange(array);
        }
        if (TxData.Count == 0)
        {
            Logger.Log("empty vibration TxData - not sending");
            return;
        }
        BS_TxMessage[] Messages = { CreateMessage(BS_VibrationMessageType.TriggerVibration, TxData) };
        SendTxMessages?.Invoke(Messages, true);
    }
}
