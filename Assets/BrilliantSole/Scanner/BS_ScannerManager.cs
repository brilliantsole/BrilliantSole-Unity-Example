using UnityEngine;

public class BS_ScannerManager : BS_SingletonMonoBehavior<BS_ScannerManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerManager", BS_Logger.LogLevel.Log);

    private static readonly BS_BaseScanner Scanner = BS_BleScanner.Instance;

    public bool IsScanning => Scanner.IsScanning;
    public void StartScan()
    {
        Scanner.StartScan();
    }
    public void StopScan()
    {
        Scanner.StopScan();
    }
    public void ToggleScan()
    {
        Scanner.ToggleScan();
    }

    public void Update()
    {
        Scanner.Update();
    }
}
