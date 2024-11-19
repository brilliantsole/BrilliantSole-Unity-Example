using UnityEngine;

public abstract class BS_BaseScanner<T> where T : BS_BaseScanner<T>, new()
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseScanner", BS_Logger.LogLevel.Log);

    private static readonly BS_ScannerSO ScannerSO = BS_ScannerSO.Instance;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    protected virtual void Initialize() { }

    protected BS_BaseScanner() { }

    public static void DestroyInstance()
    {
        _instance = null;
    }

    public virtual bool IsAvailable
    {
        get
        {
            return false;
        }
    }

    [SerializeField]
    private bool _isScanning;
    public bool IsScanning
    {
        get => _isScanning;
        protected set
        {
            if (_isScanning != value)
            {
                Logger.Log("Updating IsScanning to {value}");
                _isScanning = value;
                // FILL - trigger isScanning event
            }
        }
    }

    public virtual void StartScan()
    {
        if (IsScanning)
        {
            Logger.Log("Already scanning");
            return;
        }
        Logger.Log("Starting scan.");
    }

    public virtual void StopScan()
    {
        if (!IsScanning)
        {
            Logger.Log("Already not scanning");
            return;
        }
        Logger.Log("Stopping scan");
    }

    public void ToggleScan()
    {
        Logger.Log("Toggling scan");

        if (IsScanning)
        {
            StopScan();
        }
        else
        {
            StartScan();
        }
    }
}