using System;
using UnityEngine.Events;

public partial class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>, IBS_ScannerManager
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager");

    [Serializable]
    public class ScannerUnityEvent : UnityEvent<IBS_Scanner> { }

    [Serializable]
    public class ScannerBoolUnityEvent : UnityEvent<IBS_Scanner, bool> { }

    public BS_BaseScanner Scanner => BS_BleScanner.Instance;

    public void Update() { Scanner.Update(); }

    private void OnEnable()
    {
        Scanner.OnIsScanning += onIsScanning;
        Scanner.OnScanStart += onScanStart;
        Scanner.OnScanStop += onScanStop;

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
            OnScanStop?.Invoke(Scanner);
            OnIsScanning?.Invoke(Scanner, false);
            Scanner.StopScan();
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
