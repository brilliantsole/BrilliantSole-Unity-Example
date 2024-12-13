using UnityEngine;
using static BS_ConnectionStatus;

public partial class BS_UdpClient
{
    [SerializeField]
    private string _serverIp = "127.0.0.1";
    public string ServerIp
    {
        get => _serverIp;
        set
        {
            if (value == _serverIp)
            {
                Logger.Log($"redundant serverIp address {value}");
                return;
            }
            if (ConnectionStatus != NotConnected)
            {
                Logger.Log($"can only set serverIp address when not connected");
                return;
            }
            _serverIp = value;
            Logger.Log($"updated ServerIp to {ServerIp}");
        }
    }

    [SerializeField]
    private int _sendingPort = 3000;
    public int SendingPort
    {
        get => _sendingPort;
        set
        {
            if (value == _sendingPort)
            {
                Logger.Log($"redundant sendingPort address {value}");
                return;
            }
            if (ConnectionStatus != NotConnected)
            {
                Logger.Log($"can only set sendingPort address when not connected");
                return;
            }
            _sendingPort = value;
            Logger.Log($"updated SendingPort to {SendingPort}");
        }
    }

    [SerializeField]
    private int _listeningPort = 3001;
    public int ListeningPort
    {
        get => _listeningPort;
        set
        {
            if (value == _listeningPort)
            {
                Logger.Log($"redundant listeningPort address {value}");
                return;
            }
            if (ConnectionStatus != NotConnected)
            {
                Logger.Log($"can only set listeningPort address when not connected");
                return;
            }
            _listeningPort = value;
            Logger.Log($"updated ListeningPort to {ListeningPort}");
        }
    }
}
