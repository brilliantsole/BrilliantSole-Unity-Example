public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    public ScannerBoolUnityEvent OnIsScanning;

    public ScannerUnityEvent OnScanStart;
    public ScannerUnityEvent OnScanStop;
    private void onIsScanning(IBS_Scanner scanner, bool IsScanning) { OnIsScanning?.Invoke(Scanner, IsScanning); }
    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(Scanner); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(Scanner); }

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }
}
