using UnityEngine;
using UnityEngine.UI;

public class BS_BleScanner : BS_BaseScanner<BS_BleScanner>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger(typeof(BS_BleScanner).Name, BS_Logger.LogLevel.Log);

    public override bool IsAvailable
    {
        get
        {
            return true;
        }
    }

    public override void StartScan()
    {
        base.StartScan();
    }

    public override void StopScan()
    {
        base.StopScan();
    }
}
