using System;
using UnityEngine;
using UnityEngine.Events;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    [Serializable]
    public class ScannerUnityEvent : UnityEvent<IBS_Scanner> { }

    [Serializable]
    public class ScannerBoolUnityEvent : UnityEvent<IBS_Scanner, bool> { }

    public ScannerBoolUnityEvent OnIsScanning;
    public ScannerBoolUnityEvent OnIsScanningAvailable;

    public ScannerUnityEvent OnScanStart;
    public ScannerUnityEvent OnScanStop;

    private void onScanStart(IBS_Scanner scanner) { OnScanStart?.Invoke(scanner); }
    private void onScanStop(IBS_Scanner scanner) { OnScanStop?.Invoke(scanner); }

    public bool IsScanning => Client.IsScanning;
    public bool IsScanningAvailable => Client.IsScanningAvailable;

    private void onIsScanningAvailable(IBS_Scanner scanner, bool isScanningAvailable) { OnIsScanningAvailable?.Invoke(scanner, isScanningAvailable); }
    private void onIsScanning(IBS_Scanner scanner, bool isScanning) { OnIsScanning?.Invoke(scanner, isScanning); }

    public void StartScan() { Client.StartScan(); }
    public void StopScan() { Client.StopScan(); }
    public void ToggleScan() { Client.ToggleScan(); }
}
