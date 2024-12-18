using UnityEngine;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    public bool IsScanning => Scanner.IsScanning;

    [field: SerializeField]
    public ScannerBoolUnityEvent OnIsScanning { get; private set; } = new();

    [field: SerializeField]
    public ScannerUnityEvent OnScanStart { get; private set; } = new();
    [field: SerializeField]
    public ScannerUnityEvent OnScanStop { get; private set; } = new();

    private void onIsScanning(IBS_Scanner scanner, bool isScanning) { OnIsScanning?.Invoke(scanner, isScanning); }
    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(scanner); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(scanner); }

    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }

    public bool IsScanningAvailable => isActiveAndEnabled && Scanner.IsScanningAvailable;

    [field: SerializeField]
    public ScannerBoolUnityEvent OnIsScanningAvailable { get; private set; } = new();

    [field: SerializeField]
    public ScannerUnityEvent OnScanningIsAvailable { get; private set; } = new();
    [field: SerializeField]
    public ScannerUnityEvent OnScanningIsUnavailable { get; private set; } = new();

    private void onIsScanningAvailable(IBS_Scanner scanner, bool isScanningAvailable) { OnIsScanningAvailable?.Invoke(scanner, isScanningAvailable); }
    private void onScanningIsAvailable(IBS_Scanner scanner) { OnScanningIsAvailable?.Invoke(scanner); }
    private void onScanningIsUnavailable(IBS_Scanner scanner) { OnScanningIsUnavailable?.Invoke(scanner); }
}
