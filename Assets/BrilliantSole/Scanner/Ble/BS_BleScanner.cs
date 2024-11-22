using UnityEngine;
using UnityEngine.UI;

public class BS_BleScanner : BS_BaseScanner<BS_BleScanner>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BleScanner", BS_Logger.LogLevel.Log);

    public override bool IsAvailable
    {
        get
        {
            return true;
        }
    }

    private bool _isInitialized = false;
    private void InitializeBle()
    {
        if (_isInitialized)
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
        _isInitialized = true;
        _scanTimeout = _scanInterval;
        StartScan();
    }
    private void OnBleInitializationError(string error)
    {
        Logger.LogError($"Initialization error: {error}");
        _isInitialized = false;
        StopScan();

        if (error.Contains("Bluetooth LE Not Enabled"))
        {
            BluetoothLEHardwareInterface.BluetoothEnable(true);
        }
    }

    private void DeInitializeBle()
    {
        if (!_isInitialized)
        {
            Logger.LogWarning("Ble already not initialized");
            return;
        }
        Logger.Log("deinitializing Ble");
        BluetoothLEHardwareInterface.DeInitialize(OnBleDeInitialization);
    }
    private void OnBleDeInitialization()
    {
        Logger.Log("Ble deInitialized");
        _isInitialized = false;
        StopScan();
    }

    public override bool StartScan()
    {
        if (!base.StartScan())
        {
            return false;
        }
        if (!_isInitialized)
        {
            InitializeBle();
            return false;
        }
        else
        {
            IsScanning = true;
            return true;
        }
    }

    public override bool StopScan()
    {
        if (!base.StopScan())
        {
            return false;
        }
        BluetoothLEHardwareInterface.StopScan();
        IsScanning = false;
        return true;
    }

    private readonly float _scanInterval = 0.5f;
    private float _scanTimeout = 0;
    private void Scan()
    {
        _scanTimeout -= Time.deltaTime;
        if (_scanTimeout > 0)
        {
            return;
        }
        _scanTimeout = _scanInterval;
        Logger.Log("Scanning for devices...");
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(BS_BleUtils.ServiceUuids, OnDiscoveredBleDevice, OnDiscoveredBleDeviceData, true);
    }

    private void OnDiscoveredBleDevice(string address, string name)
    {
        Logger.Log($"Discovered device \"{name}\" with address {address}");
        AddDiscoveredDevice(new BS_DiscoveredDevice(name, 0, address));
    }
    private void OnDiscoveredBleDeviceData(string address, string name, int rssi, byte[] bytes)
    {
        Logger.Log($"Discovered device \"{name}\" with address {address}, RSSI {rssi}, and {bytes.Length} bytes");
        AddDiscoveredDevice(new BS_DiscoveredDevice(name, rssi, address));
    }

    public override void Update()
    {
        base.Update();

        if (IsScanning)
        {
            Scan();
        }

        // FILL - connection
    }

    protected override void DeInitialize()
    {
        base.DeInitialize();
        DeInitializeBle();
    }

    public override BS_Device ConnectToDiscoveredDevice(BS_DiscoveredDevice DiscoveredDevice)
    {
        BS_Device Device = base.ConnectToDiscoveredDevice(DiscoveredDevice);
        // FILL
        return Device;
    }
}
