using UnityEngine;

public partial class BS_BaseClient
{
    [SerializeField]
    private bool _isScanningAvailable = false;
    public event IBS_Scanner.IsScanningAvailableDelegate OnIsScanningAvailable;
    public event IBS_Scanner.ScannerDelegate OnScanningIsAvailable;
    public event IBS_Scanner.ScannerDelegate OnScanningIsUnavailable;
    public bool IsScanningAvailable
    {
        get => _isScanningAvailable;
        private set
        {
            if (value == _isScanningAvailable)
            {
                Logger.Log($"redundant IsScanningAvailable {value}");
                return;
            }
            _isScanningAvailable = value;
            Logger.Log($"updated IsScanningAvailable to {IsScanningAvailable}");
            OnIsScanningAvailable?.Invoke(this, IsScanningAvailable);

            if (IsScanningAvailable)
            {
                Logger.Log("checking if is scanning...");
                SendMessages(new() { new(BS_ServerMessageType.IsScanning) });
            }

            if (IsScanningAvailable)
            {
                OnScanningIsAvailable?.Invoke(this);
            }
            else
            {
                OnScanningIsUnavailable?.Invoke(this);
            }
        }
    }
    private void ParseIsScanningAvailable(in byte[] data)
    {
        var newIsScanningAvailable = data[0] == 1;
        Logger.Log($"newIsScanningAvailable: {newIsScanningAvailable}");
        IsScanningAvailable = newIsScanningAvailable;
    }

    [SerializeField]
    private bool _isScanning = false;
    public event IBS_Scanner.IsScanningDelegate OnIsScanning;
    public event IBS_Scanner.ScannerDelegate OnScanStart;
    public event IBS_Scanner.ScannerDelegate OnScanStop;
    public bool IsScanning
    {
        get => _isScanning;
        private set
        {
            if (value == _isScanning)
            {
                Logger.Log($"redundant IsScanning {value}");
                return;
            }
            _isScanning = value;
            Logger.Log($"updated IsScanning to {IsScanning}");
            OnIsScanning?.Invoke(this, IsScanning);
            if (IsScanning)
            {
                OnScanStart?.Invoke(this);
            }
            else
            {
                OnScanStop?.Invoke(this);
            }
        }
    }
    private void ParseIsScanning(in byte[] data)
    {
        var newIsScanning = data[0] == 1;
        Logger.Log($"newIsScanning: {newIsScanning}");
        IsScanning = newIsScanning;
    }

    public bool StartScan()
    {
        if (!IsScanningAvailable)
        {
            Logger.Log("scanning is not available");
            return false;
        }
        if (IsScanning)
        {
            Logger.Log("already scanning");
            return false;
        }
        _discoveredDevices.Clear();
        SendMessages(new() { new(BS_ServerMessageType.StartScan) });
        return true;
    }
    public bool StopScan()
    {
        if (!IsScanning)
        {
            Logger.Log("already not scanning");
            return false;
        }
        SendMessages(new() { new(BS_ServerMessageType.StopScan) });
        return true;
    }
    public void ToggleScan() { if (IsScanning) { StopScan(); } else { StartScan(); } }
}
