using UnityEngine;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    public bool IsScanning => Scanner.IsScanning;

    [field: SerializeField]
    public ScannerManagerBoolUnityEvent OnIsScanning { get; private set; } = new();

    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanStart { get; private set; } = new();
    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanStop { get; private set; } = new();

    private void onIsScanning(IBS_Scanner scanner, bool isScanning) { OnIsScanning?.Invoke(this, isScanning); }
    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(this); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(this); }

    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }

    public bool IsScanningAvailable => isActiveAndEnabled && Scanner.IsScanningAvailable;

    [field: SerializeField]
    public ScannerManagerBoolUnityEvent OnIsScanningAvailable { get; private set; } = new();

    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanningIsAvailable { get; private set; } = new();
    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanningIsUnavailable { get; private set; } = new();

    private void onIsScanningAvailable(IBS_Scanner scanner, bool isScanningAvailable) { OnIsScanningAvailable?.Invoke(this, isScanningAvailable); }
    private void onScanningIsAvailable(IBS_Scanner scanner) { OnScanningIsAvailable?.Invoke(this); }
    private void onScanningIsUnavailable(IBS_Scanner scanner) { OnScanningIsUnavailable?.Invoke(this); }
}
