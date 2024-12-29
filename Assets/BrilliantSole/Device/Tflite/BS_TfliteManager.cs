using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BS_TfliteMessageType;
using static BS_TfliteTask;

public class BS_TfliteManager : BS_BaseManager<BS_TfliteMessageType>
{
    public static readonly BS_TfliteMessageType[] RequiredMessageTypes = {
        GetTfliteName,
        GetTfliteTask,
        GetTfliteSensorRate,
        GetTfliteSensorTypes,
        IsTfliteReady,
        GetTfliteCaptureDelay,
        GetTfliteThreshold,
        GetTfliteInferencingEnabled,
    };
    public static byte[] RequiredTxRxMessageTypes => EnumArrayToTxRxArray(RequiredMessageTypes);

    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_TfliteManager");

    public override void OnRxMessage(BS_TfliteMessageType messageType, in byte[] data)
    {
        base.OnRxMessage(messageType, data);
        switch (messageType)
        {
            case GetTfliteName:
            case SetTfliteName:
                ParseName(data);
                break;
            case GetTfliteTask:
            case SetTfliteTask:
                ParseTask(data);
                break;
            case GetTfliteSensorRate:
            case SetTfliteSensorRate:
                ParseSensorRate(data);
                break;
            case GetTfliteSensorTypes:
            case SetTfliteSensorTypes:
                ParseSensorTypes(data);
                break;
            case IsTfliteReady:
                ParseIsReady(data);
                break;
            case GetTfliteCaptureDelay:
            case SetTfliteCaptureDelay:
                ParseCaptureDelay(data);
                break;
            case GetTfliteThreshold:
            case SetTfliteThreshold:
                ParseThreshold(data);
                break;
            case GetTfliteInferencingEnabled:
            case SetTfliteInferencingEnabled:
                ParseInferencingEnabled(data);
                break;
            case TfliteInference:
                ParseInference(data);
                break;
            default:
                Logger.LogError($"uncaught messageType {messageType}");
                break;
        }
    }

    public override void Reset()
    {
        base.Reset();

        _name = null;
        _task = null;
        _sensorRate = null;
        _sensorTypes = null;
        _isReady = null;
        _captureDelay = null;
        _threshold = null;
        _inferencingEnabled = null;
        _tfliteModelMetadata = null;
    }

    private BS_TfliteModelMetadata _tfliteModelMetadata;
    public BS_TfliteModelMetadata TfliteModelMetadata
    {
        get => _tfliteModelMetadata;
        private set
        {
            if (_tfliteModelMetadata == value)
            {
                Logger.Log($"redundant TfliteModelMetadata assignment");
                return;
            }
            Logger.Log($"updated TfliteModelMetadata");
            _tfliteModelMetadata = value;
        }
    }

    public void SendTfliteModel(BS_TfliteModelMetadata tfliteModelMetadata, bool sendImmediately = true)
    {
        // FILL
        TfliteModelMetadata = tfliteModelMetadata;
        SetName(tfliteModelMetadata.Name, false);
        SetTask(tfliteModelMetadata.Task, false);
        SetCaptureDelay(tfliteModelMetadata.CaptureDelay, false);
        SetSensorRate(tfliteModelMetadata.SensorRate, false);
        SetThreshold(tfliteModelMetadata.Threshold, false);
        SetSensorTypes(tfliteModelMetadata.GetSensorTypes(), sendImmediately);
    }

    // NAME START
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
        string name = BS_StringUtils.GetString(data);
        Logger.Log($"parsed name: {name}");
        Name = name;
    }
    private void SetName(string newName, bool sendImmediately = true)
    {
        if (newName == Name)
        {
            Logger.Log($"redundant name {newName}");
            return;
        }
        Logger.Log($"setting name to {newName}...");

        List<byte> data = new(BS_StringUtils.ToBytes(newName));
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteName, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // NAME END

    // TASK START
    [SerializeField]
    private BS_TfliteTask? _task;
    public BS_TfliteTask Task
    {
        get => _task ?? Classification;
        private set
        {
            if (_task == value) { return; }
            Logger.Log($"Updating Task to {value}");
            _task = value;
            OnTask?.Invoke(Task);
        }
    }
    public event Action<BS_TfliteTask> OnTask;
    private void ParseTask(in byte[] data)
    {
        var task = (BS_TfliteTask)data[0];
        Logger.Log($"parsed task: {task}");
        Task = task;
    }
    private void SetTask(BS_TfliteTask newTask, bool sendImmediately = true)
    {
        if (newTask == Task)
        {
            Logger.Log($"redundant fileType {newTask}");
            return;
        }
        Logger.Log($"setting fileType to {newTask}...");

        List<byte> data = new() { (byte)newTask };
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteTask, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // TASK END

    // SENSOR RATE START
    [SerializeField]
    private BS_SensorRate? _sensorRate;
    public BS_SensorRate SensorRate
    {
        get => _sensorRate ?? BS_SensorRate._0ms;
        private set
        {
            if (_sensorRate == value) { return; }
            Logger.Log($"Updating SensorRate to {value}");
            _sensorRate = value;
            OnSensorRate?.Invoke(SensorRate);
        }
    }
    public event Action<BS_SensorRate> OnSensorRate;
    private void ParseSensorRate(in byte[] data)
    {
        var rawSensorRate = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"parsed rawSensorRate: {rawSensorRate}");
        var sensorRate = BS_SensorConfigurationManager.GetClosestSensorRate(rawSensorRate);
        Logger.Log($"parsed sensorRate: {sensorRate}");
        SensorRate = sensorRate;
    }
    private void SetSensorRate(BS_SensorRate newSensorRate, bool sendImmediately = true)
    {
        if (newSensorRate == SensorRate)
        {
            Logger.Log($"redundant sensorRate {newSensorRate}");
            return;
        }
        Logger.Log($"setting sensorRate to {newSensorRate}...");

        List<byte> data = new();
        data.AddRange(BS_ByteUtils.ToByteArray((ushort)newSensorRate, true));
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteSensorRate, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // SENSOR RATE END

    // SENSOR TYPES START
    [SerializeField]
    private HashSet<BS_SensorType> _sensorTypes;
    public HashSet<BS_SensorType> SensorTypes
    {
        get => _sensorTypes;
        private set
        {
            if (_sensorTypes == value) { return; }
            Logger.Log($"Updating SensorTypes to {value}");
            _sensorTypes = value;
            OnSensorTypes?.Invoke(SensorTypes);
        }
    }
    public event Action<HashSet<BS_SensorType>> OnSensorTypes;
    private void ParseSensorTypes(in byte[] data)
    {
        HashSet<BS_SensorType> sensorTypes = new();
        foreach (var rawSensorType in data)
        {
            var sensorType = (BS_SensorType)rawSensorType;
            Logger.Log($"adding sensorType {sensorType}");
            sensorTypes.Add(sensorType);
        }
        Logger.Log($"parsed sensorTypes: {string.Join(", ", sensorTypes)}");
        SensorTypes = sensorTypes;
    }
    private void SetSensorTypes(HashSet<BS_SensorType> newSensorTypes, bool sendImmediately = true)
    {
        if (SensorTypes.SetEquals(newSensorTypes))
        {
            Logger.Log($"redundant SensorTypes assignment");
            return;
        }
        Logger.Log($"setting sensorTypes to {string.Join(", ", newSensorTypes)}...");
        List<byte> data = newSensorTypes.Select(e => (byte)e).ToList();
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteSensorTypes, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // SENSOR TYPES END

    // IS READY START
    [SerializeField]
    private bool? _isReady;
    public bool IsReady
    {
        get => _isReady ?? false;
        private set
        {
            if (_isReady == value) { return; }
            Logger.Log($"Updating IsReady to {value}");
            _isReady = value;
            OnIsReady?.Invoke(IsReady);
        }
    }
    public event Action<bool> OnIsReady;
    private void ParseIsReady(in byte[] data)
    {
        var isReady = data[0] == 1;
        Logger.Log($"parsed isReady: {isReady}");
        IsReady = isReady;
    }
    // IS READY END

    // CAPTURE DELAY START
    [SerializeField]
    private ushort? _captureDelay;
    public ushort CaptureDelay
    {
        get => _captureDelay ?? 0;
        private set
        {
            if (_captureDelay == value) { return; }
            Logger.Log($"Updating CaptureDelay to {value}");
            _captureDelay = value;
            OnCaptureDelay?.Invoke(CaptureDelay);
        }
    }
    public event Action<ushort> OnCaptureDelay;
    private void ParseCaptureDelay(in byte[] data)
    {
        var captureDelay = BS_ByteUtils.ParseNumber<ushort>(data, isLittleEndian: true);
        Logger.Log($"parsed captureDelay: {captureDelay}");
        CaptureDelay = captureDelay;
    }
    private void SetCaptureDelay(ushort newCaptureDelay, bool sendImmediately = true)
    {
        if (newCaptureDelay == CaptureDelay)
        {
            Logger.Log($"redundant captureDelay {newCaptureDelay}");
            return;
        }
        Logger.Log($"setting captureDelay to {newCaptureDelay}...");

        List<byte> data = BS_ByteUtils.ToByteArray(newCaptureDelay, true);
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteCaptureDelay, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // CAPTURE DELAY END

    // THRESHOLD START
    [SerializeField]
    private float? _threshold;
    public float Threshold
    {
        get => _threshold ?? 0;
        private set
        {
            if (_threshold == value) { return; }
            Logger.Log($"Updating Threshold to {value}");
            _threshold = value;
            OnThreshold?.Invoke(Threshold);
        }
    }
    public event Action<float> OnThreshold;
    private void ParseThreshold(in byte[] data)
    {
        var threshold = BS_ByteUtils.ParseNumber<float>(data, isLittleEndian: true);
        Logger.Log($"parsed threshold: {threshold}");
        Threshold = threshold;
    }
    private void SetThreshold(float newThreshold, bool sendImmediately = true)
    {
        if (newThreshold == Threshold)
        {
            Logger.Log($"redundant threshold {newThreshold}");
            return;
        }
        Logger.Log($"setting threshold to {newThreshold}...");

        List<byte> data = BS_ByteUtils.ToByteArray(newThreshold, true);
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteThreshold, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    // THRESHOLD END

    // INFERENCING ENABLED START
    [SerializeField]
    private bool? _inferencingEnabled;
    public bool InferencingEnabled
    {
        get => _inferencingEnabled ?? false;
        private set
        {
            if (_inferencingEnabled == value) { return; }
            Logger.Log($"Updating InferencingEnabled to {value}");
            _inferencingEnabled = value;
            OnInferencingEnabled?.Invoke(InferencingEnabled);
        }
    }
    public event Action<bool> OnInferencingEnabled;
    private void ParseInferencingEnabled(in byte[] data)
    {
        var inferencingEnabled = data[0] == 1;
        Logger.Log($"parsed inferencingEnabled: {inferencingEnabled}");
        InferencingEnabled = inferencingEnabled;
    }
    public void SetInferencingEnabled(bool newInferencingEnabled, bool sendImmediately = true)
    {
        if (newInferencingEnabled == InferencingEnabled)
        {
            Logger.Log($"redundant inferencingEnabled {newInferencingEnabled}");
            return;
        }
        Logger.Log($"setting inferencingEnabled to {newInferencingEnabled}...");

        List<byte> data = new() { (byte)(newInferencingEnabled ? 1 : 0) };
        BS_TxMessage[] Messages = { CreateMessage(SetTfliteInferencingEnabled, data) };
        SendTxMessages(Messages, sendImmediately);
    }
    public void ToggleInferencingEnabled() { SetInferencingEnabled(!InferencingEnabled); }
    // INFERENCING ENABLED END

    // INFERENCE START
    public event Action<List<float>, Dictionary<string, float>, ulong> OnInference;
    public event Action<string, float, ulong> OnClassification;
    private void ParseInference(in byte[] data)
    {
        int offset = 0;

        var timestamp = BS_TimeUtils.ParseTimestamp(data, offset);
        Logger.Log($"timestamp: {timestamp}");
        offset += 2;

        var inferenceMessageLength = data.Length - offset;
        var inferenceSize = sizeof(float);
        if (inferenceMessageLength % inferenceSize != 0)
        {
            Logger.LogError($"inferenceMessageLength length is not a multiple of {inferenceSize} (got {inferenceMessageLength})");
            return;
        }
        var numberOfInferences = inferenceMessageLength / inferenceSize;

        Dictionary<string, float> inferenceMap = null;
        if (TfliteModelMetadata != null)
        {
            if (TfliteModelMetadata.Classes?.Count == numberOfInferences)
            {
                inferenceMap = new();
            }
            else
            {
                Logger.LogError($"TfliteModelMetadata classes doesn't match (expected {numberOfInferences}, got {TfliteModelMetadata.Classes.Count})");
            }
        }
        else
        {
            Logger.LogWarning("null TfliteModelMetadata");
        }


        List<float> inference = new();
        var maxValue = float.MinValue;
        var maxIndex = -1;
        string maxClassName = null;
        for (int i = 0; offset < data.Length; offset += inferenceSize, i++)
        {
            var value = BS_ByteUtils.ParseNumber<float>(data, offset, true);
            Logger.Log($"class #{i} value: {value}");
            inference.Add(value);

            if (inferenceMap != null)
            {
                var className = TfliteModelMetadata.Classes[i];
                inferenceMap[className] = value;
                Logger.Log($"#{i} {className}: {value}");
                if (Task == Classification)
                {
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxIndex = i;
                        maxClassName = className;
                    }
                }
            }
        }
        Logger.Log($"parsed inference with {inference.Count} classes as {timestamp}ms");

        OnInference?.Invoke(inference, inferenceMap, timestamp);
        if (Task == Classification && maxClassName != null)
        {
            Logger.Log($"max class: {maxClassName} (#{maxIndex}) with {maxValue}");
            OnClassification?.Invoke(maxClassName, maxValue, timestamp);
        }
    }
    // INFERENCE END
}
