public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>, IBS_ScannerManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager");

    public IBS_Scanner Scanner => BS_BleScanner.Instance;

    public void Update() { Scanner.Update(); }

    private void OnEnable()
    {
        Scanner.OnIsScanning += onIsScanning;
        Scanner.OnScanStart += onScanStart;
        Scanner.OnScanStop += onScanStop;

        Scanner.OnIsScanningAvailable += onIsScanningAvailable;
        Scanner.OnScanningIsAvailable += onScanningIsAvailable;
        Scanner.OnScanningIsUnavailable += onScanningIsUnavailable;

        Scanner.OnDiscoveredDevice += onDiscoveredDevice;
        Scanner.OnExpiredDevice += onExpiredDevice;
    }

    private void OnDisable()
    {
        Scanner.OnIsScanning -= onIsScanning;
        Scanner.OnScanStart -= onScanStart;
        Scanner.OnScanStop -= onScanStop;
        if (IsScanning)
        {
            OnScanStop?.Invoke(this);
            OnIsScanning?.Invoke(this, false);
            Scanner.StopScan();
        }

        Scanner.OnIsScanningAvailable -= onIsScanningAvailable;
        Scanner.OnScanningIsAvailable -= onScanningIsAvailable;
        Scanner.OnScanningIsUnavailable -= onScanningIsUnavailable;

        if (IsScanningAvailable)
        {
            OnScanningIsUnavailable?.Invoke(this);
            OnIsScanningAvailable?.Invoke(this, false);
        }

        Scanner.OnDiscoveredDevice -= onDiscoveredDevice;
        Scanner.OnExpiredDevice -= onExpiredDevice;
    }

#if UNITY_EDITOR
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        BS_BleScanner.DestroyInstance();
    }
#endif
}
