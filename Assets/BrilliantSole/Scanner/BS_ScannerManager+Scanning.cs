using System;
using UnityEngine.Events;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    [Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    public BoolUnityEvent OnIsScanning;

    public UnityEvent OnScanStart;
    public UnityEvent OnScanStop;
    private void onIsScanning(bool IsScanning) { OnIsScanning?.Invoke(IsScanning); }
    private void onScanStart() { OnScanStart?.Invoke(); }
    private void onScanStop() { OnScanStop?.Invoke(); }

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }
}
