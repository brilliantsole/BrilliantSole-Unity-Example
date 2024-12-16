using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClientManager", BS_Logger.LogLevel.Log);
    protected virtual TClient Client => null;
    public void Update() { Client.Update(); }

    protected virtual void OnEnable()
    {
        Client.OnConnectionStatus += onConnectionStatus;
        Client.OnIsConnected += onIsConnected;

        Client.OnIsScanningAvailable += onIsScanningAvailable;
        Client.OnIsScanning += onIsScanning;

        Client.OnDiscoveredDevice += onDiscoveredDevice;
        Client.OnExpiredDevice += onExpiredDevice;
    }

    protected virtual void OnDisable()
    {
        Client.OnConnectionStatus -= onConnectionStatus;
        Client.OnIsConnected -= onIsConnected;

        Client.OnIsScanningAvailable -= onIsScanningAvailable;
        Client.OnIsScanning -= onIsScanning;

        Client.OnDiscoveredDevice -= onDiscoveredDevice;
        Client.OnExpiredDevice -= onExpiredDevice;
    }
}