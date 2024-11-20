using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager", BS_Logger.LogLevel.Log);

    private static readonly BS_BaseScanner Scanner = BS_BleScanner.Instance;

    [System.Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    public BoolUnityEvent OnIsScanning;

    public UnityEvent OnScanStart;
    public UnityEvent OnScanStop;

    private void OnEnable()
    {
        Scanner.OnIsScanning += _OnIsScanning;
        Scanner.OnScanStart += _OnScanStart;
        Scanner.OnScanStop += _OnScanStop;
    }

    private void OnDisable()
    {
        Scanner.OnIsScanning -= _OnIsScanning;
        Scanner.OnScanStart -= _OnScanStart;
        Scanner.OnScanStop -= _OnScanStop;
    }

    private void _OnIsScanning(bool IsScanning) { OnIsScanning?.Invoke(IsScanning); }
    private void _OnScanStart() { OnScanStart?.Invoke(); }
    private void _OnScanStop() { OnScanStop?.Invoke(); }

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan() { Scanner.StartScan(); }
    public void StopScan() { Scanner.StopScan(); }
    public void ToggleScan() { Scanner.ToggleScan(); }

    public void Update() { Scanner.Update(); }
}
