using System.Collections.Generic;

public partial class BS_Device
{
    private readonly List<BS_TxMessage> PendingTxMessages = new();
    private readonly List<byte> TxData = new();
    private bool IsSendingTxData = false;

    private void OnSendTxData(BS_BaseConnectionManager connectionManager)
    {
        Logger.Log("Sent Tx Data");
        IsSendingTxData = false;
        foreach (var BaseManager in Managers) { BaseManager.OnSendTxData(); }
        SendPendingTxMessages();
    }

    private void SendTxMessages(BS_TxMessage[] txMessages, bool sendImmediately = true)
    {
        Logger.Log($"Requesting to send {txMessages.Length} messages...");
        PendingTxMessages.AddRange(txMessages);
        if (!sendImmediately)
        {
            Logger.Log("Not sending data immediately");
            return;
        }
        if (IsSendingTxData)
        {
            Logger.Log("Already sending TxData - will wait until new data is sent");
            return;
        }
        SendPendingTxMessages();
    }

    private void SendPendingTxMessages()
    {
        if (PendingTxMessages.Count == 0) { return; }
        IsSendingTxData = true;
        TxData.Clear();

        var maxMessageLength = InformationManager.MaxTxMessageLength;

        byte pendingTxMessageIndex = 0;
        while (pendingTxMessageIndex < PendingTxMessages.Count)
        {
            var pendingTxMessage = PendingTxMessages[pendingTxMessageIndex];
            var pendingTxMessageLength = pendingTxMessage.Length();
            var pendingTxMessageType = BS_TxRxMessageUtils.EnumStrings[pendingTxMessage.Type];
            bool shouldAppendTxMessage = maxMessageLength == 0 || TxData.Count + pendingTxMessageLength <= maxMessageLength;
            if (shouldAppendTxMessage)
            {
                Logger.Log($"appending message \"{pendingTxMessageType}\" ({pendingTxMessageLength} bytes)");
                pendingTxMessage.AppendTo(TxData);
                PendingTxMessages.RemoveAt(pendingTxMessageIndex);
            }
            else
            {
                Logger.Log($"skipping message \"{pendingTxMessageType}\" ({pendingTxMessageLength} bytes)");
                pendingTxMessageIndex++;
            }
        }

        if (TxData.Count == 0)
        {
            Logger.Log("TxData is empty - no data to send");
            IsSendingTxData = false;
            return;
        }
        Logger.Log($"Sending {TxData.Count} Tx bytes...");
        SendTxData(TxData);
    }

    private void SendTxData(List<byte> Data) { ConnectionManager?.SendTxData(Data); }

    private void ResetTxMessaging()
    {
        IsSendingTxData = false;
        PendingTxMessages.Clear();
    }
}
