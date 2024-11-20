using System;
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

    public event Action<bool> OnIsScanning;

    public event Action OnScanStart;
    public event Action OnScanStop;

    [SerializeField]
    private bool _isScanning;
    public bool IsScanning
    {
        get => _isScanning;
        protected set
        {
            if (_isScanning != value)
            {
                Logger.Log($"Updating IsScanning to {value}");
                _isScanning = value;
                OnIsScanning?.Invoke(IsScanning);
                if (IsScanning)
                {
                    OnScanStart?.Invoke();
                }
                else
                {
                    OnScanStop?.Invoke();
                }
            }
        }
    }

    public virtual bool StartScan()
    {
        if (IsScanning)
        {
            Logger.Log("Already scanning");
            return false; ;
        }
        if (!IsAvailable)
        {
            Logger.LogError("Scanning is not available");
            return false;
        }
        Logger.Log("Starting scan.");
        return true;
    }

    public virtual bool StopScan()
    {
        if (!IsScanning)
        {
            Logger.Log("Already not scanning");
            return false;
        }
        Logger.Log("Stopping scan");
        return true;
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
    protected virtual void DeInitialize() { }

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
        _instance?.DeInitialize();
        _instance = null;
    }

    protected BS_BaseScanner() { }
}
