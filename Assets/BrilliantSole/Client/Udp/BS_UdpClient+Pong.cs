public partial class BS_UdpClient
{
    private void Pong()
    {
        Logger.Log("Ponging server...");
        SendUdpMessages(new() { new(BS_UdpMessageType.Pong) });
    }

    private void WaitForPong()
    {
        // FILL
    }

    private void StopWaitingForPong()
    {
        // FILL
    }

    private void PongTimeout()
    {
        // FILL
    }
}
