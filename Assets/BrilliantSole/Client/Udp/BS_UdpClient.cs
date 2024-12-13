using System.Net.Sockets;

public partial class BS_UdpClient : BS_BaseClient<BS_UdpClient>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClient", BS_Logger.LogLevel.Log);

    private UdpClient UdpClient;
    private bool IsRunning = false;
}