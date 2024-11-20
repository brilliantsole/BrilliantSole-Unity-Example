using UnityEngine;
using UnityEngine.UI;

public class BS_BleScanner : BS_BaseScanner<BS_BleScanner>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger(typeof(BS_BleScanner).Name, BS_Logger.LogLevel.Log);

    public override bool IsAvailable
    {
        get
        {
            // FILL - check if BLE is available
            return true;
        }
    }

    private bool isInitialized = false;
    private void InitializeBle()
    {
        if (isInitialized)
        {
            Logger.LogWarning("Ble already initialized");
            return;
        }
        Logger.Log("initializing Ble");
        BluetoothLEHardwareInterface.Initialize(true, false, OnBleInitializationSuccess, OnBleInitializationError);
    }

    private void OnBleInitializationSuccess()
    {
        Logger.Log("Ble initialized");
        isInitialized = true;
        StartScan();
    }
    private void OnBleInitializationError(string error)
    {
        Logger.LogError($"Initialization error: {error}");
        isInitialized = false;
        StopScan();
    }

    public override void StartScan()
    {
        base.StartScan();
        if (!isInitialized)
        {
            InitializeBle();
        }
        else
        {
            // FILL
        }
    }

    public override void StopScan()
    {
        base.StopScan();
    }

    public override void Update()
    {
        base.Update();
        // FILL
        Logger.Log("update ;/");
    }
}
