using UnityEngine;

#nullable enable

public class BS_Device
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseConnectionManager", BS_Logger.LogLevel.Log);

    // CONNECTION MANAGER START
    private BS_BaseConnectionManager? _connectionManager;
    public BS_BaseConnectionManager? ConnectionManager
    {
        get => _connectionManager;
        set
        {
            if (_connectionManager != value)
            {
                if (_connectionManager != null)
                {
                    // FILL - remove listeners
                    _connectionManager.OnStatus = null;
                }
                // FILL - add listeners
                _connectionManager = value;
            }
        }
    }

    public BS_ConnectionType? ConnectionType => ConnectionManager?.Type;
    public BS_ConnectionStatus ConnectionStatus => ConnectionManager?.Status ?? BS_ConnectionStatus.NotConnected;
    public void Connect() { ConnectionManager?.Connect(); }
    public void Disconnect() { ConnectionManager?.Disconnect(); }
    // CONNECTION MANAGER END
}
