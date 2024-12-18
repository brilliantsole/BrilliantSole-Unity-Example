using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public bool IsScanning => Client.IsScanning;

    [field: SerializeField]
    public ScannerBoolUnityEvent OnIsScanning { get; private set; } = new();

    [field: SerializeField]
    public ScannerUnityEvent OnScanStart { get; private set; } = new();
    [field: SerializeField]
    public ScannerUnityEvent OnScanStop { get; private set; } = new();

    private void onIsScanning(IBS_Scanner scanner, bool isScanning) { OnIsScanning?.Invoke(scanner, isScanning); }
    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(scanner); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(scanner); }

    public void StartScan() { Client.StartScan(); }
    public void StopScan() { Client.StopScan(); }
    public void ToggleScan() { Client.ToggleScan(); }

    public bool IsScanningAvailable => Client.IsScanningAvailable;

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
