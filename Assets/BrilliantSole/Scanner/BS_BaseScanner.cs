using UnityEngine;

public abstract class BS_BaseScanner
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseScanner", BS_Logger.LogLevel.Log);

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
        if (!IsAvailable)
        {
            Logger.LogError("Scanning is not available");
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

    protected virtual void Initialize() { }

    public virtual void Update() { }
}

public abstract class BS_BaseScanner<T> : BS_BaseScanner where T : BS_BaseScanner<T>, new()
{
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

    public static void DestroyInstance()
    {
        _instance = null;
    }

    protected BS_BaseScanner() { }
}
