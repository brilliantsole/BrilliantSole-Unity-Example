using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public bool IsScanning => Client.IsScanning;

    [field: SerializeField]
    public ScannerManagerBoolUnityEvent OnIsScanning { get; private set; } = new();

    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanStart { get; private set; } = new();
    [field: SerializeField]
    public ScannerManagerUnityEvent OnScanStop { get; private set; } = new();

    private void onIsScanning(IBS_Scanner scanner, bool isScanning) { OnIsScanning?.Invoke(this, isScanning); }
    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(this); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(this); }

    public void StartScan() { Client.StartScan(); }
    public void StopScan() { Client.StopScan(); }
    public void ToggleScan() { Client.ToggleScan(); }

    public bool IsScanningAvailable => Client.IsScanningAvailable;

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
