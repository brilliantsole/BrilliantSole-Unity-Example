using System;
using UnityEngine;

public partial class BS_BaseClient
{
    [SerializeField]
    private bool _isScanningAvailable = false;
    public event Action<BS_BaseClient, bool> OnIsScanningAvailable;
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
    public event Action<BS_BaseClient, bool> OnIsScanning;
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
        }
    }
    private void ParseIsScanning(in byte[] data)
    {
        var newIsScanning = data[0] == 1;
        Logger.Log($"newIsScanning: {newIsScanning}");
        IsScanning = newIsScanning;
    }

    public void StartScan()
    {
        if (!IsScanningAvailable)
        {
            Logger.Log("scanning is not available");
            return;
        }
        if (IsScanning)
        {
            Logger.Log("already scanning");
            return;
        }
        _discoveredDevices.Clear();
        SendMessages(new() { new(BS_ServerMessageType.StartScan) });
    }
    public void StopScan()
    {
        if (!IsScanning)
        {
            Logger.Log("already not scanning");
            return;
        }
        SendMessages(new() { new(BS_ServerMessageType.StopScan) });
    }
    public void ToggleScan() { if (IsScanning) { StopScan(); } else { StartScan(); } }
}
