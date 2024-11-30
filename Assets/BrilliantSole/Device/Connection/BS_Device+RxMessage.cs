public partial class BS_Device
{
    private void OnRxMessage(BS_BaseConnectionManager connectionManager, byte messageTypeEnum, byte[] messageData)
    {
        Logger.Log($"Received messageType \"{BS_TxRxMessageUtils.EnumStrings[messageTypeEnum]}\"");
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
        if (ConnectionStatus == BS_ConnectionStatus.Connecting)
        {
            // FILL - check if fully connected
        }
    }
}
