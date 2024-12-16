using System.Net;

public partial class BS_UdpClientManager : BS_BaseClientManager<BS_UdpClientManager, BS_UdpClient>
{
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

    public int SendPort => Client.SendPort;
    public void SetSendPort(int newSendPort) { Client.SendPort = newSendPort; }
    public void SetSendPort(string newSendPortString)
    {
        var newSendPort = ParsePortString(newSendPortString);
        if (newSendPort != null) { SetSendPort((int)newSendPort); }
    }

    public int ReceivePort => Client.ReceivePort;
    public void SetReceivePort(int newReceivePort) { Client.ReceivePort = newReceivePort; }
    public void SetReceivePort(string newReceivePortString)
    {
        var newReceivePort = ParsePortString(newReceivePortString);
        if (newReceivePort != null) { SetSendPort((int)newReceivePort); }
    }

}
