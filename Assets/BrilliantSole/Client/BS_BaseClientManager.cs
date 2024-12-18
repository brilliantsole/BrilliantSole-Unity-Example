using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>, IBS_ScannerManager
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClientManager", BS_Logger.LogLevel.Log);
    protected virtual TClient Client => null;
    public void Update() { Client.Update(); }

    public IBS_Scanner Scanner => Client;

    protected virtual void OnEnable()
    {
        Client.OnConnectionStatus += onConnectionStatus;
        Client.OnIsConnected += onIsConnected;

        Client.OnIsScanning += onIsScanning;
        Client.OnScanStart += onScanStart;
        Client.OnScanStop += onScanStop;

        Client.OnIsScanningAvailable += onIsScanningAvailable;
        Client.OnScanningIsAvailable += onScanningIsAvailable;
        Client.OnScanningIsUnavailable += onScanningIsUnavailable;

        Client.OnDiscoveredDevice += onDiscoveredDevice;
        Client.OnExpiredDevice += onExpiredDevice;
    }

    protected virtual void OnDisable()
    {
        Client.OnConnectionStatus -= onConnectionStatus;
        Client.OnIsConnected -= onIsConnected;

        Client.OnIsScanning -= onIsScanning;
        Client.OnScanStart -= onScanStart;
        Client.OnScanStop -= onScanStop;
        if (IsScanning)
        {
            OnScanStop?.Invoke(this);
            OnIsScanning?.Invoke(this, false);
            Client.StopScan();
        }

        Client.OnIsScanningAvailable -= onIsScanningAvailable;
        Client.OnScanningIsAvailable -= onScanningIsAvailable;
        Client.OnScanningIsUnavailable -= onScanningIsUnavailable;

        if (IsScanningAvailable)
        {
            OnScanningIsUnavailable?.Invoke(this);
            OnIsScanningAvailable?.Invoke(this, false);
        }

        Client.OnDiscoveredDevice -= onDiscoveredDevice;
        Client.OnExpiredDevice -= onExpiredDevice;
    }
}