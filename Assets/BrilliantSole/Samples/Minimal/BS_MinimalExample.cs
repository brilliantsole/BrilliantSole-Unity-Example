using UnityEngine;
using static BS_SensorType;
using static BS_SensorRate;
using static BS_VibrationWaveformEffect;
using System.Collections.Generic;
using System.Linq;

public class BS_MinimalExample : MonoBehaviour
{
    // helper logger
    static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_MinimalExample", BS_Logger.LogLevel.Log);

    // helper global class instance to listen for insole events without manually adding/removing listeners on individual devices on connection/disconnection
    BS_DevicePair DevicePair => BS_DevicePair.Insoles;

    // for scanning+connecting to devices via bluetooth (if the BLE Plugin is available for your system)
    BS_BleScanner BleScanner => BS_BleScanner.Instance;

    // for managing a connecting to the Node.js server via UDP (optional if the Ble Plugin isn't available for your system)
    BS_UdpClientManager UdpClientManager => BS_UdpClientManager.Instance;

    // ENABLE/DISABLE
    void OnEnable()
    {
        AddScannerListeners(BleScanner);
        AddScannerListeners(UdpClientManager.Scanner);
        AddClientListeners(UdpClientManager.Client);
        AddDevicePairListeners();
    }
    void OnDisable()
    {
        RemoveScannerListeners(BleScanner);
        RemoveScannerListeners(UdpClientManager.Scanner);
        RemoveClientListeners(UdpClientManager.Client);
        RemoveDevicePairListeners();
    }

    void StopScan()
    {
        BleScanner.StopScan();
        UdpClientManager.Scanner.StopScan();
    }

    // SCANNER LISTENERS
    void AddScannerListeners(IBS_Scanner Scanner)
    {
        Scanner.OnIsScanningAvailable += OnIsScanningAvailable;
        Scanner.OnScanStart += OnScanStart;
        Scanner.OnScanStop += OnScanStop;
        Scanner.OnDiscoveredDevice += OnDiscoveredDevice;
    }
    void RemoveScannerListeners(IBS_Scanner Scanner)
    {
        Scanner.OnIsScanningAvailable -= OnIsScanningAvailable;
        Scanner.OnScanStart -= OnScanStart;
        Scanner.OnScanStop -= OnScanStop;
        Scanner.OnDiscoveredDevice -= OnDiscoveredDevice;
    }

    void OnIsScanningAvailable(IBS_Scanner scanner, bool isScanningAvailable)
    {
        Logger.Log($"OnIsScanningAvailable \"{isScanningAvailable}\"");
        if (isScanningAvailable)
        {
            scanner.StartScan();
        }
    }
    void OnScanStart(IBS_Scanner scanner)
    {
        Logger.Log("OnScanStart");
    }
    void OnScanStop(IBS_Scanner scanner)
    {
        Logger.Log("OnScanStop");
    }
    void OnDiscoveredDevice(BS_DiscoveredDevice discoveredDevice)
    {
        Logger.Log($"OnDiscoveredDevice \"{discoveredDevice.Name}\"");

        Logger.Log($"connecting to device \"{discoveredDevice.Name}\"...");
        discoveredDevice.Connect();
    }

    // CLIENT LISTENERS
    void AddClientListeners(BS_BaseClient BaseClient)
    {
        BaseClient.OnConnected += OnClientConnected;
        BaseClient.OnNotConnected += OnClientNotConnected;
    }
    void RemoveClientListeners(BS_BaseClient BaseClient)
    {
        BaseClient.OnConnected -= OnClientConnected;
        BaseClient.OnNotConnected -= OnClientNotConnected;
    }

    void OnClientConnected(BS_BaseClient client)
    {
        Logger.Log("OnClientConnected");

        Logger.Log("starting scan...");
        client.StartScan();
    }
    void OnClientNotConnected(BS_BaseClient client)
    {
        Logger.Log($"OnClientNotConnected");
    }

    // DEVICE PAIR LISTENERS
    void AddDevicePairListeners()
    {
        DevicePair.OnDeviceConnected += OnDeviceConnected;
        DevicePair.OnDeviceNotConnected += OnDeviceNotConnected;

        DevicePair.OnDeviceSensorConfiguration += OnDeviceSensorConfiguration;

        DevicePair.OnDeviceLinearAcceleration += OnDeviceLinearAcceleration;
        DevicePair.OnDeviceGameRotation += OnDeviceGameRotation;
        DevicePair.OnDeviceRotation += OnDeviceRotation;
        DevicePair.OnDeviceOrientation += OnDeviceOrientation;
        DevicePair.OnDeviceGyroscope += OnDeviceGyroscope;
        DevicePair.OnDevicePressureData += OnDevicePressureData;

        DevicePair.OnDeviceFileTransferStatus += OnDeviceFileTransferStatus;
        DevicePair.OnDeviceFileTransferProgress += OnDeviceFileTransferProgress;
        DevicePair.OnDeviceFileTransferComplete += OnDeviceFileTransferComplete;

        DevicePair.OnDeviceTfliteReady += OnDeviceTfliteReady;
        DevicePair.OnIsDeviceTfliteInferencingEnabled += OnIsDeviceTfliteInferencingEnabled;
        DevicePair.OnDeviceTfliteInference += OnDeviceTfliteInference;
        DevicePair.OnDeviceTfliteClassification += OnDeviceTfliteClassification;
    }
    void RemoveDevicePairListeners()
    {
        DevicePair.OnDeviceConnected -= OnDeviceConnected;
        DevicePair.OnDeviceNotConnected -= OnDeviceNotConnected;

        DevicePair.OnDeviceSensorConfiguration -= OnDeviceSensorConfiguration;

        DevicePair.OnDeviceLinearAcceleration -= OnDeviceLinearAcceleration;
        DevicePair.OnDeviceGameRotation -= OnDeviceGameRotation;
        DevicePair.OnDeviceRotation -= OnDeviceRotation;
        DevicePair.OnDeviceOrientation -= OnDeviceOrientation;
        DevicePair.OnDeviceGyroscope -= OnDeviceGyroscope;
        DevicePair.OnDevicePressureData -= OnDevicePressureData;

        DevicePair.OnDeviceFileTransferStatus -= OnDeviceFileTransferStatus;
        DevicePair.OnDeviceFileTransferProgress -= OnDeviceFileTransferProgress;
        DevicePair.OnDeviceFileTransferComplete -= OnDeviceFileTransferComplete;

        DevicePair.OnDeviceTfliteReady -= OnDeviceTfliteReady;
        DevicePair.OnIsDeviceTfliteInferencingEnabled -= OnIsDeviceTfliteInferencingEnabled;
        DevicePair.OnDeviceTfliteInference -= OnDeviceTfliteInference;
        DevicePair.OnDeviceTfliteClassification -= OnDeviceTfliteClassification;
    }

    public BS_SensorConfiguration sensorConfiguration = new() {
        {LinearAcceleration, _0ms},
        {GameRotation, _0ms},
        {Rotation, _0ms},
        {Orientation, _0ms},
        {BS_SensorType.Gyroscope, _0ms},
        {Pressure, _0ms},
    };

    void OnDeviceConnected(BS_DevicePair pair, BS_Side side, BS_Device device)
    {
        Logger.Log($"{side} {pair.Type} device connected");
        Logger.Log("stopping scan...");
        StopScan();

        device.SetSensorConfiguration(sensorConfiguration);
    }
    void OnDeviceNotConnected(BS_DevicePair pair, BS_Side side, BS_Device device)
    {
        Logger.Log($"{side} {pair.Type} device not connected");
    }

    void OnDeviceSensorConfiguration(BS_DevicePair pair, BS_Side side, BS_Device device, BS_SensorConfiguration configuration)
    {
        Logger.Log($"{side} {pair.Type} device sensorConfiguration updated:\n{configuration}");
    }

    // DEVICE PAIR SENSOR DATA
    void OnDeviceLinearAcceleration(BS_DevicePair pair, BS_Side side, BS_Device device, Vector3 vector, ulong timestamp)
    {
        Logger.Log($"[{timestamp}] OnDeviceLinearAcceleration {side} {pair.Type}, {vector}");
    }
    void OnDeviceGameRotation(BS_DevicePair pair, BS_Side side, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        Logger.Log($"[{timestamp}] OnDeviceGameRotation {side} {pair.Type}, {quaternion.eulerAngles}");
    }
    void OnDeviceRotation(BS_DevicePair pair, BS_Side side, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        Logger.Log($"[{timestamp}] OnDeviceRotation {side} {pair.Type}: {quaternion.eulerAngles}");
    }
    void OnDeviceOrientation(BS_DevicePair pair, BS_Side side, BS_Device device, Vector3 vector, ulong timestamp)
    {
        Logger.Log($"[{timestamp}] OnDeviceOrientation {side} {pair.Type}, {vector}");
    }
    void OnDeviceGyroscope(BS_DevicePair pair, BS_Side side, BS_Device device, Vector3 vector, ulong timestamp)
    {
        Logger.Log($"[{timestamp}] OnDeviceGyroscope {side} {pair.Type}: {vector}");
    }
    void OnDevicePressureData(BS_DevicePair pair, BS_Side side, BS_Device device, BS_PressureData pressureData, ulong timestamp)
    {
        var pressureDataString = string.Join(", ", pressureData.Sensors.Select(sensor => $"{sensor.NormalizedValue}"));
        Logger.Log($"[{timestamp}] OnDevicePressureData {side} {pair.Type}: {pressureDataString}");
    }

    // DEVICE PAIR FILE TRANSFER

    void OnDeviceFileTransferStatus(BS_DevicePair pair, BS_Side side, BS_Device device, BS_FileTransferStatus status)
    {
        Logger.Log($"OnDeviceFileTransferStatus {side} {pair.Type}: {status}");
    }
    void OnDeviceFileTransferProgress(BS_DevicePair pair, BS_Side side, BS_Device device, BS_FileType type, BS_FileTransferDirection direction, float progress)
    {
        Logger.Log($"OnDeviceFileTransferProgress {side} {pair.Type}: {progress}%");
    }
    void OnDeviceFileTransferComplete(BS_DevicePair pair, BS_Side side, BS_Device device, BS_FileType type, BS_FileTransferDirection direction)
    {
        Logger.Log($"OnDeviceFileTransferComplete {side} {pair.Type}");
    }

    // DEVICE PAIR TFLITE

    private void OnDeviceTfliteReady(BS_DevicePair pair, BS_Side side, BS_Device device)
    {
        Logger.Log($"OnDeviceTfliteReady {side} {pair.Type}");

        Logger.Log($"enabling tflite inferencing on \"{device.Name}\"...");
        tfliteInferencingEnabled = true;
        device.SetTfliteInferencingEnabled(tfliteInferencingEnabled);
    }

    private void OnIsDeviceTfliteInferencingEnabled(BS_DevicePair pair, BS_Side side, BS_Device device, bool isInferencngEnabled)
    {
        Logger.Log($"OnIsDeviceTfliteInferencingEnabled {side} {pair.Type}: {isInferencngEnabled}");
    }

    private void OnDeviceTfliteInference(BS_DevicePair pair, BS_Side side, BS_Device device, List<float> inference, Dictionary<string, float> inferenceMap, ulong timestamp)
    {
        Logger.Log($"OnDeviceTfliteInference {side} {pair.Type}:");

        var inferenceString = string.Join(", ", inferenceMap.Select(pair => $"{pair.Key}: {pair.Value}"));
        Logger.Log($"[{timestamp}] OnDeviceTfliteInference {side} {pair.Type}: {inferenceString}");
    }

    private void OnDeviceTfliteClassification(BS_DevicePair pair, BS_Side side, BS_Device device, string className, float classValue, ulong timestamp)
    {
        Logger.Log($"OnDeviceTfliteInference {side} {pair.Type}: \"{className}\" with value {classValue}");
    }

    // START/UPDATE
    void Start()
    {
        // can change "true" to "false" to test udp if ble is available
        if (true && BleScanner.IsScanningAvailable)
        {
            Logger.Log("Scanning is available via Ble");
            BleScanner.StartScan();
        }
        else
        {
            // default ip address - uncomment if using a different server on the server
            // UdpClientManager.SetServerIp("127.0.0.1");

            // default send/receive ports - no need to change them
            // UdpClientManager.SetSendPort(3000);
            // UdpClientManager.SetReceivePort(3001);
            UdpClientManager.Connect();
        }
    }

    public BS_SensorRate SensorRate = _20ms;

    public List<BS_VibrationConfiguration> vibrationConfiguration = new()
    {
        new () {
            Locations = BS_VibrationLocationFlag.Front | BS_VibrationLocationFlag.Rear,
            Type = BS_VibrationType.WaveformEffect,
            WaveformEffectSequence = new() {new(StrongClick_100)}
        }
    };

    public BS_TfliteModelMetadata TfliteModelMetadata;

    bool tfliteInferencingEnabled = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Logger.Log("Toggling Linear Acceleration...");
            DevicePair.ToggleSensorRate(LinearAcceleration, SensorRate);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Logger.Log("Toggling Game Rotation...");
            DevicePair.ToggleSensorRate(GameRotation, SensorRate);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Logger.Log("Toggling Rotation...");
            DevicePair.ToggleSensorRate(Rotation, SensorRate);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Logger.Log("Toggling Orientation...");
            DevicePair.ToggleSensorRate(Orientation, SensorRate);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Logger.Log("Toggling Gyroscope...");
            DevicePair.ToggleSensorRate(BS_SensorType.Gyroscope, SensorRate);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Logger.Log("Toggling Pressure...");
            DevicePair.ToggleSensorRate(Pressure, SensorRate);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Logger.Log("Triggering Vibration...");
            DevicePair.TriggerVibration(vibrationConfiguration);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Logger.Log("Uploading tflite model...");
            DevicePair.SendTfliteModel(TfliteModelMetadata);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            tfliteInferencingEnabled = !tfliteInferencingEnabled;
            Logger.Log($"setting tfliteInferencingEnabled {tfliteInferencingEnabled}...");
            DevicePair.SetTfliteInferencingEnabled(tfliteInferencingEnabled);
        }
    }
}
