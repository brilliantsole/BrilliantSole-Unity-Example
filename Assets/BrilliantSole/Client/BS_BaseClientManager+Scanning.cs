using UnityEngine;
using UnityEngine.Events;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public BoolUnityEvent OnIsScanning;
    public BoolUnityEvent OnIsScanningAvailable;



    public UnityEvent OnScanStart;
    public UnityEvent OnScanStop;

    private void onScanStart(BS_BaseClient client) { OnScanStart?.Invoke(); }
    private void onScanStop(BS_BaseClient client) { OnScanStop?.Invoke(); }

    public bool IsScanning => Client.IsScanning;
    public bool IsScanningAvailable => Client.IsScanningAvailable;

    private void onIsScanningAvailable(BS_BaseClient client, bool isScanningAvailable) { OnIsScanningAvailable?.Invoke(isScanningAvailable); }
    private void onIsScanning(BS_BaseClient client, bool isScanning) { OnIsScanning?.Invoke(isScanning); }

    public void StartScan() { Client.StartScan(); }
    public void StopScan() { Client.StopScan(); }
    public void ToggleScan() { Client.ToggleScan(); }
}
