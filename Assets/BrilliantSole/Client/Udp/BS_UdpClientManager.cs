using System.Net;

public class BS_UdpClientManager : BS_SingletonMonoBehavior<BS_UdpClientManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClientManager", BS_Logger.LogLevel.Log);
    private static BS_UdpClient Client => BS_UdpClient.Instance;

    public string ServerIp => Client.ServerIp;
    public void SetServerIp(string newServerIp)
    {
        if (!IPAddress.TryParse(newServerIp, out _))
        {
            Logger.LogError($"invalid ip address {newServerIp}");
            return;
        }

        Client.ServerIp = newServerIp;
    }

    private int? ParsePortString(string portString)
    {
        if (int.TryParse(portString, out int port) && port >= 0 && port <= 65535)
        {
            return port;
        }
        else
        {
            Logger.LogError($"invalid port {portString}");
            return null;
        }
    }

    public int SendingPort => Client.SendingPort;
    public void SetSendingPort(int newSendingPort) { Client.SendingPort = newSendingPort; }
    public void SetSendingPort(string newSendingPortString)
    {
        var newSendingPort = ParsePortString(newSendingPortString);
        if (newSendingPort != null) { SetSendingPort((int)newSendingPort); }
    }

    public int ListeningPort => Client.ListeningPort;
    public void SetListeningPort(int newListeningPort) { Client.ListeningPort = newListeningPort; }
    public void SetListeningPort(string newListeningPortString)
    {
        var newListeningPort = ParsePortString(newListeningPortString);
        if (newListeningPort != null) { SetSendingPort((int)newListeningPort); }
    }

    public void ToggleConnection() { Client.ToggleConnection(); }
    public void Connect() { Client.Connect(); }
    public void Disconnect() { Client.Disconnect(); }

    private void OnEnable()
    {
        // Add Listeners
    }

    private void OnDisable()
    {
        // Remove Listeners
    }
}
