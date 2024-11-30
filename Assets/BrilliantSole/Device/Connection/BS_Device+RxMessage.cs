using System.Collections.Generic;
using static BS_ConnectionStatus;

public partial class BS_Device
{
    private readonly HashSet<byte> ReceivedTxRxMessages = new();

    private void OnRxMessage(BS_BaseConnectionManager connectionManager, byte messageTypeEnum, byte[] messageData)
    {
        Logger.Log($"Received messageType \"{BS_TxRxMessageUtils.EnumStrings[messageTypeEnum]}\"");
        ReceivedTxRxMessages.Add(messageTypeEnum);
        foreach (var BaseManager in Managers)
        {
            if (BaseManager.OnRxMessage(messageTypeEnum, messageData))
            {
                break;
            }
        }
    }
    private void OnRxMessages(BS_BaseConnectionManager connectionManager)
    {
        Logger.Log("Parsed Rx Messages");
        SendPendingTxMessages();
        if (ConnectionStatus == Connecting)
        {
            CheckIfFullyConnected();
        }
    }

    private void CheckIfFullyConnected()
    {
        Logger.Log("Checking if fully connected...");
        if (_batteryLevel == null)
        {
            Logger.Log("Didn't get battery level - stopping now...");
            return;
        }
        if (!DeviceInformation.HasAllInformation)
        {
            Logger.Log("Don't have all DeviceInformation - stopping now...");
            return;
        }
        if (ConnectionStatus != Connecting)
        {
            Logger.Log("ConnectionStatus is not connecting - stopping now...");
            return;
        }
        if (InformationManager.CurrentTime == 0)
        {
            Logger.Log("Current time is 0 - stopping now...");
            return;
        }

        bool receivedAllRequiredTxRxMessages = CheckIfReceivedAllRequiredTxRxMessages();
        Logger.Log($"receivedAllRequiredTxRxMessages? {receivedAllRequiredTxRxMessages}");
        if (!receivedAllRequiredTxRxMessages) { return; }
        ConnectionStatus = Connected;
    }

    private bool CheckIfReceivedAllRequiredTxRxMessages()
    {
        bool receivedAllRequiredTxRxMessages = true;
        foreach (var requiredTxRxMessageType in BS_TxRxMessageUtils.RequiredTxRxMessageTypes)
        {
            if (!ReceivedTxRxMessages.Contains(requiredTxRxMessageType))
            {
                Logger.Log($"Didn't receive message \"{BS_TxRxMessageUtils.EnumStrings[requiredTxRxMessageType]}\"");
                receivedAllRequiredTxRxMessages = false;
                break;
            }
        }
        return receivedAllRequiredTxRxMessages;
    }

    private void ResetRxMessaging()
    {
        ReceivedTxRxMessages.Clear();
    }
}
