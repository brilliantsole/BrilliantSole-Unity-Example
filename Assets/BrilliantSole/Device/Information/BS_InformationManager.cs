using System;
using System.Text;
using UnityEngine;
using static BS_InformationMessageType;

public class BS_InformationManager : BS_BaseManager<BS_InformationMessageType>
{
    public static readonly BS_InformationMessageType[] RequiredMessageTypes = {
        GetMtu,
        GetId,
        GetName,
        GetDeviceType,
        GetCurrentTime
     };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_InformationManager");

    public override void OnRxMessage(BS_InformationMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetMtu:
                ParseMtu(data);
                break;
            case GetId:
                ParseId(data);
                break;
            case GetName:
            case SetName:
                ParseName(data);
                break;
            case GetDeviceType:
            case SetDeviceType:
                ParseDeviceType(data);
                break;
            case GetCurrentTime:
            case SetCurrentTime:
                ParseCurrentTime(data);
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    [SerializeField]
    private ushort? _mtu;
    public ushort Mtu
    {
        get => _mtu ?? 0;
        private set
        {
            if (_mtu == value) { return; }
            Logger.Log($"Updating Mtu to {value}");
            _mtu = value;
            OnMtu?.Invoke(Mtu);
        }
    }
    public ushort MaxTxMessageLength => (ushort)(Mtu == 0 ? 0 : Mtu - 3);
    public event Action<ushort> OnMtu;
    private void ParseMtu(in byte[] data)
    {
        var mtu = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"Parsed mtu: {mtu}");
        Mtu = mtu;
    }

    [SerializeField]
    private string _id;
    public string Id
    {
        get => _id;
        private set
        {
            if (_id == value) { return; }
            Logger.Log($"Updating Id to {value}");
            _id = value;
            OnId?.Invoke(Id);
        }
    }
    public event Action<string> OnId;
    private void ParseId(in byte[] data)
    {
        string id = Encoding.UTF8.GetString(data);
        Logger.Log($"parsed id: {id}");
        Id = id;
    }

    [SerializeField]
    private string _name;
    public string Name
    {
        get => _name;
        private set
        {
            if (_name == value) { return; }
            Logger.Log($"Updating Name to {value}");
            _name = value;
            OnName?.Invoke(Name);
        }
    }
    public event Action<string> OnName;
    private void ParseName(in byte[] data)
    {
        string name = Encoding.UTF8.GetString(data);
        Logger.Log($"parsed name: {name}");
        Name = name;
    }

    [SerializeField]
    private BS_DeviceType? _deviceType;
    public BS_DeviceType DeviceType
    {
        get => _deviceType ?? BS_DeviceType.LeftInsole;
        private set
        {
            if (_deviceType == value) { return; }
            Logger.Log($"Updating DeviceType to {value}");
            _deviceType = value;
            OnDeviceType?.Invoke(DeviceType);
        }
    }
    public event Action<BS_DeviceType> OnDeviceType;
    private void ParseDeviceType(in byte[] data)
    {
        BS_DeviceType deviceType = (BS_DeviceType)data[0];
        Logger.Log($"parsed deviceType: {deviceType}");
        DeviceType = deviceType;
    }

    [SerializeField]
    private ulong? _currentTime;
    public ulong CurrentTime
    {
        get => _currentTime ?? 0;
        private set
        {
            if (_currentTime == value) { return; }
            Logger.Log($"Updating CurrentTime to {value}");
            _currentTime = value;
            if (CurrentTime == 0)
            {
                UpdateCurrentTime();
            }
            else
            {
                OnCurrentTime?.Invoke(CurrentTime);
            }
        }
    }
    private void UpdateCurrentTime()
    {
        var currentTime = BS_TimeUtils.GetMilliseconds();
        Logger.Log($"Updating CurrentTime to {currentTime}");
        BS_TxMessage[] Messages = { CreateTxMessage(SetCurrentTime, BS_ByteUtils.ToByteArray(currentTime, true)) };
        SendTxMessages?.Invoke(Messages, false);
    }
    public event Action<ulong> OnCurrentTime;
    private void ParseCurrentTime(in byte[] data)
    {
        var currentTime = BS_ByteUtils.ParseNumber<ulong>(data, isLittleEndian: true);
        Logger.Log($"parsed currentTime: {currentTime}");
        CurrentTime = currentTime;
    }

    public override void Reset()
    {
        base.Reset();

        _mtu = null;
        _id = null;
        _name = null;
        _deviceType = null;
        _currentTime = null;
    }
}
