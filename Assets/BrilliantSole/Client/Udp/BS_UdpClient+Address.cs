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
    private int _sendPort = 3000;
    public int SendPort
    {
        get => _sendPort;
        set
        {
            if (value == _sendPort)
            {
                Logger.Log($"redundant sendPort address {value}");
                return;
            }
            if (ConnectionStatus != NotConnected)
            {
                Logger.Log($"can only set sendPort address when not connected");
                return;
            }
            _sendPort = value;
            Logger.Log($"updated SendPort to {SendPort}");
        }
    }

    [SerializeField]
    private int _receivePort = 3001;
    public int ReceivePort
    {
        get => _receivePort;
        set
        {
            if (value == _receivePort)
            {
                Logger.Log($"redundant listeningPort address {value}");
                return;
            }
            if (ConnectionStatus != NotConnected)
            {
                Logger.Log($"can only set listeningPort address when not connected");
                return;
            }
            _receivePort = value;
            Logger.Log($"updated ListeningPort to {ReceivePort}");
        }
    }
}
