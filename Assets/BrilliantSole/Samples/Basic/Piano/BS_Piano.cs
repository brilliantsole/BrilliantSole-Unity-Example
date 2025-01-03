using System;
using System.Collections.Generic;
using jp.kshoji.unity.midi;
using MidiPlayerTK;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BS_Piano : MonoBehaviour, IMidiDeviceEventHandler, IMidiAllEventsHandler
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_Piano", BS_Logger.LogLevel.Log);

    public BS_InsoleSide InsoleSide = BS_InsoleSide.Right;
    public BS_SensorRate SensorRate = BS_SensorRate._40ms;
    private BS_DevicePair DevicePair => BS_DevicePair.Instance;
    private BS_Device Device => DevicePair.Devices.ContainsKey(InsoleSide) ? DevicePair.Devices[InsoleSide] : null;
    private bool IsInsoleConnected => Device?.IsConnected == true;

    public GameObject InstrumentsGrid;
    public List<string> InstrumentsGridNames = new();
    public GameObject InstrumentGridButton;

    public BS_PianoUI PianoUI;

    public enum BS_PedalMode
    {
        None,
        Sustain,
        Reverb,
        Chorus,
        Drum
    }

    [SerializeField]
    private BS_PedalMode pedalMode = BS_PedalMode.None;
    public BS_PedalMode PedalMode
    {
        get => pedalMode;
        private set
        {
            if (value == pedalMode) { return; }
            pedalMode = value;
            OnPedalMode();
        }
    }
    private void OnPedalMode()
    {
        Logger.Log($"updated pedal mode to \"{PedalMode}\"");

        SustainText.transform.parent.gameObject.SetActive(PedalMode == BS_PedalMode.Sustain);

        switch (PedalMode)
        {
            case BS_PedalMode.None:
                Device?.ClearSensorRate(BS_SensorType.GameRotation);
                break;
            default:
                Device?.SetSensorRate(BS_SensorType.GameRotation, SensorRate);
                break;
        }

        Reset();
    }

    public TMP_Dropdown PedalModeDropdown;
    public Button CalibrateButton;
    public TextMeshProUGUI SustainText;

    public TMP_Dropdown ModeDropdown;

    private MidiStreamPlayer midiStreamPlayer;

    private void Log(string message)
    {
        Logger.Log(message);
    }
    private void LogWarning(string message)
    {
        Logger.LogWarning(message);
    }
    private void LogError(string message)
    {
        Logger.LogError(message);
    }

    public TMP_Dropdown InstrumentDropdown;

    private void Awake()
    {
        MidiManager.Instance.RegisterEventHandleObject(gameObject);
        MidiManager.Instance.InitializeMidi(() =>
        {
            Logger.Log("initialized midi");
        });

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif

        midiStreamPlayer = FindAnyObjectByType<MidiStreamPlayer>();
        if (midiStreamPlayer != null)
        {
            midiStreamPlayer.OnEventSynthAwake.AddListener(OnEventSynthAwake);
            midiStreamPlayer.OnEventSynthStarted.AddListener(OnEventSynthStarted);
        }
        else
        {
            LogError("unable to find MidiStreamPlayer");
        }
    }

    private void Start()
    {
        MidiPlayerGlobal.OnEventPresetLoaded.AddListener(OnEventPresetLoaded);

        PopulateInstrumentDropdown();
        InstrumentDropdown.onValueChanged.AddListener(OnInstrumentDropdownValueChanged);
        PedalModeDropdown.onValueChanged.AddListener(OnPedalModeDropdownValueChanged);

        PedalModeDropdown.ClearOptions();
        List<string> PedalModeStrings = new(Enum.GetNames(typeof(BS_PedalMode)));
        PedalModeDropdown.AddOptions(PedalModeStrings);

        ModeDropdown.onValueChanged.AddListener(OnModeDropdownValueChanged);

        CalibrateButton.onClick.AddListener(Calibrate);

        PopulateInstrumentsGrid();

        PianoUI.OnKeyDown += OnKeyDown;
        PianoUI.OnKeyUp += OnKeyUp;
    }
    private void OnEnable()
    {
        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;
        OnPedalMode();
        Reset();
    }
    private void OnDisable()
    {
        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;
        if (!gameObject.scene.isLoaded) return;
        Device?.ClearSensorRate(BS_SensorType.GameRotation);
    }

    public enum BS_Mode
    {
        Play,
        Track
    }
    [SerializeField]
    private BS_Mode mode = BS_Mode.Play;
    public BS_Mode Mode
    {
        get => mode;
        private set
        {
            if (value == mode) { return; }
            mode = value;
            OnMode();
        }
    }
    private void OnMode()
    {
        Logger.Log($"updated mode to \"{Mode}\"");

        switch (Mode)
        {
            case BS_Mode.Play:
                // FILL
                break;
            case BS_Mode.Track:
                // FILL
                break;
        }

    }

    private void OnModeDropdownValueChanged(int selectedIndex)
    {
        var selectedModeString = ModeDropdown.options[selectedIndex].text;
        //Logger.Log($"selectedModeString: {selectedModeString}");

        if (Enum.TryParse(selectedModeString, out BS_Mode selectedMode))
        {
            //Logger.Log($"parsed selectedMode {selectedMode}");
            Mode = selectedMode;
        }
        else
        {
            Logger.LogError($"uncaught selectedModeString \"{selectedModeString}\"");
        }
    }

    private void OnKeyDown(BS_PianoKeyData KeyData)
    {
        Log($"OnKeyDown {KeyData.MidiNote}");
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = KeyData.MidiNote,
            Channel = StreamChannel,
            Duration = -1,
            Velocity = 80
        });
    }
    private void OnKeyUp(BS_PianoKeyData KeyData)
    {
        Log($"OnKeyUp {KeyData.MidiNote}");
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOff,
            Value = KeyData.MidiNote,
            Channel = StreamChannel,
        });
    }

    private void PopulateInstrumentDropdown()
    {
        InstrumentDropdown.ClearOptions();

        var instrumentList = MidiPlayerGlobal.MPTK_ListPreset;
        if (instrumentList != null)
        {
            List<string> instrumentNames = new();
            foreach (var preset in instrumentList)
            {
                instrumentNames.Add(RemoveInstrumentNumber(preset.Label));
            }
            InstrumentDropdown.AddOptions(instrumentNames);
        }
        else
        {
            LogError("No SoundFont is loaded or accessible.");
        }
    }
    private void PopulateInstrumentsGrid()
    {
        while (InstrumentsGrid.transform.childCount > 0)
        {
            var child = InstrumentsGrid.transform.GetChild(0);
            if (Application.isEditor)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        var instrumentList = MidiPlayerGlobal.MPTK_ListPreset;
        if (instrumentList != null)
        {
            foreach (var preset in instrumentList)
            {
                var instrumentName = RemoveInstrumentNumber(preset.Label);
                Logger.Log($"instrumentName: {instrumentName}");
                if (InstrumentsGridNames.Contains(instrumentName))
                {
                    var instrumentGridButton = Instantiate(InstrumentGridButton, InstrumentsGrid.transform);
                    instrumentGridButton.GetComponentInChildren<TextMeshProUGUI>().text = instrumentName;
                    instrumentGridButton.GetComponentInChildren<BS_EyeInteractable>().OnHover += (eyeInteractable) =>
                    {
                        InstrumentDropdown.value = preset.Index;
                        OnInstrumentDropdownValueChanged(preset.Index);
                    };
                }
            }
        }
        else
        {
            LogError("No SoundFont is loaded or accessible.");
        }
    }

    private string RemoveInstrumentNumber(string label)
    {
        int dashIndex = label.IndexOf(" - ");
        return dashIndex >= 0 ? label.Substring(dashIndex + 3) : label;
    }

    private int StreamChannel = 0;
    private readonly int DrumChannel = 9;
    private void OnInstrumentDropdownValueChanged(int selectedIndex)
    {
        Log($"Instrument changed to {InstrumentDropdown.options[selectedIndex].text}");

        var instrumentList = MidiPlayerGlobal.MPTK_ListPreset;
        if (instrumentList != null && selectedIndex >= 0 && selectedIndex < instrumentList.Count)
        {
            var preset = instrumentList[selectedIndex];
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.PatchChange, Value = preset.Index, Channel = StreamChannel, });

            Logger.Log($"Instrument Preset change - channel:{StreamChannel} bank:{midiStreamPlayer.MPTK_Channels[StreamChannel].BankNum} preset:{midiStreamPlayer.MPTK_Channels[StreamChannel].PresetNum}");
        }
        else
        {
            Logger.LogError("Invalid selection index or instrument list.");
        }
    }

    private void OnPedalModeDropdownValueChanged(int selectedIndex)
    {
        var selectedPedalModeString = PedalModeDropdown.options[selectedIndex].text;
        //Logger.Log($"selectedPedalModeString: {selectedPedalModeString}");

        if (Enum.TryParse(selectedPedalModeString, out BS_PedalMode selectedPedalMode))
        {
            //Logger.Log($"parsed selectedPedalMode {selectedPedalMode}");
            PedalMode = selectedPedalMode;
        }
        else
        {
            Logger.LogError($"uncaught selectedPedalModeString \"{selectedPedalModeString}\"");
        }
    }

    // MIDI PLAYER START
    private void OnEventPresetLoaded()
    {
        Log("OnEventPresetLoaded");

    }

    private void OnEventSynthAwake(string name)
    {
        Log($"OnEventSynthAwake {name}");
    }
    private void OnEventSynthStarted(string name)
    {
        Log($"OnEventSynthStarted {name}");
    }
    // MIDI PLAYER END

    private void OnDestroy()
    {
        Deinitialize();
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            Log("OnPlayModeStateChanged");
            Deinitialize();
        }
    }
#endif

    private void Deinitialize()
    {
        Log("Deinitialize");
        MidiManager.Instance.TerminateMidi();
        DestroyImmediate(MidiManager.Instance);
    }

    // MIDI KEYBOARD START
    public void OnMidiInputDeviceAttached(string deviceId)
    {
        Log($"OnMidiInputDeviceAttached {deviceId} name: {MidiManager.Instance.GetDeviceName(deviceId)}");
    }

    public void OnMidiInputDeviceDetached(string deviceId)
    {
        Log($"OnMidiInputDeviceDetached {deviceId} name: {MidiManager.Instance.GetDeviceName(deviceId)}");
    }

    public void OnMidiOutputDeviceAttached(string deviceId)
    {
        Log($"OnMidiOutputDeviceAttached {deviceId} name: {MidiManager.Instance.GetDeviceName(deviceId)}");
    }

    public void OnMidiOutputDeviceDetached(string deviceId)
    {
        Log($"OnMidiOutputDeviceDetached {deviceId} name: {MidiManager.Instance.GetDeviceName(deviceId)}");
    }

    public void OnMidiNoteOn(string deviceId, int group, int channel, int note, int velocity)
    {
        Log($"OnMidiNoteOn note: {note}, velocity: {velocity}");
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = StreamChannel,
            Velocity = velocity
        });
        PianoUI.OnMidiNote(note, true);
    }

    public void OnMidiNoteOff(string deviceId, int group, int channel, int note, int velocity)
    {
        Log($"OnMidiNoteOff note: {note}, velocity: {velocity}");
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOff,
            Value = note,
            Channel = StreamChannel,
            //Velocity = velocity
        });
        PianoUI.OnMidiNote(note, false);
    }

    public void OnMidiChannelAftertouch(string deviceId, int group, int channel, int pressure)
    {
        Log($"OnMidiChannelAftertouch pressure: {pressure}");
    }

    public void OnMidiPitchWheel(string deviceId, int group, int channel, int amount)
    {
        Log($"OnMidiPitchWheel channel: {channel}, amount: {amount}");
    }

    public void OnMidiPolyphonicAftertouch(string deviceId, int group, int channel, int note, int pressure)
    {
        Log($"OnMidiPolyphonicAftertouch note: {note}, pressure: {pressure}");
    }

    public void OnMidiProgramChange(string deviceId, int group, int channel, int program)
    {
        Log($"OnMidiProgramChange");
    }

    public void OnMidiControlChange(string deviceId, int group, int channel, int function, int value)
    {
        Log($"OnMidiControlChange function: {function}, value: {value}");
    }

    public void OnMidiContinue(string deviceId, int group)
    {
        Log($"OnMidiContinue");
    }

    public void OnMidiReset(string deviceId, int group)
    {
        Log($"OnMidiReset");
    }

    public void OnMidiStart(string deviceId, int group)
    {
        Log($"OnMidiStart");
    }

    public void OnMidiStop(string deviceId, int group)
    {
        Log($"OnMidiStop");
    }

    public void OnMidiActiveSensing(string deviceId, int group)
    {
        Log($"OnMidiActiveSensing");
    }

    public void OnMidiCableEvents(string deviceId, int group, int byte1, int byte2, int byte3)
    {
        Log($"OnMidiCableEvents");
    }

    public void OnMidiSongSelect(string deviceId, int group, int song)
    {
        Log($"OnMidiSongSelect");
    }

    public void OnMidiSongPositionPointer(string deviceId, int group, int position)
    {
        Log($"OnMidiSongPositionPointer");
    }

    public void OnMidiSingleByte(string deviceId, int group, int byte1)
    {
        Log($"OnMidiSingleByte");
    }

    public void OnMidiSystemExclusive(string deviceId, int group, byte[] systemExclusive)
    {
        Log($"OnMidiSystemExclusive");
    }

    public void OnMidiSystemCommonMessage(string deviceId, int group, byte[] message)
    {
        Log($"OnMidiSystemCommonMessage");
    }

    public void OnMidiTimeCodeQuarterFrame(string deviceId, int group, int timing)
    {
        Log($"OnMidiTimeCodeQuarterFrame");
    }

    public void OnMidiTimingClock(string deviceId, int group)
    {
        Log($"OnMidiTimingClock");
    }

    public void OnMidiTuneRequest(string deviceId, int group)
    {
        Log($"OnMidiTuneRequest");
    }

    public void OnMidiMiscellaneousFunctionCodes(string deviceId, int group, int byte1, int byte2, int byte3)
    {
        Log($"OnMidiMiscellaneousFunctionCodes");
    }
    // MIDI KEYBOARD END

    [SerializeField]
    private bool sustain = false;
    public bool Sustain
    {
        get => sustain;
        private set
        {
            if (value == sustain) { return; }
            sustain = value;
            Logger.Log($"updated sustain to {Sustain}");
            SustainText.gameObject.SetActive(Sustain);

            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
            {
                Command = MPTKCommand.ControlChange,
                Controller = MPTKController.Sustain,
                Value = !Sustain ? 0 : 64,
                Channel = StreamChannel
            });
        }
    }

    public int DrumNote = 35;
    private void PlayDrum()
    {
        Logger.Log("playing drum");
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = DrumNote,
            Channel = DrumChannel,
        });
    }

    // BS START
    private float PitchThreshold = 0.0f;
    private readonly BS_Range PitchRange = new();
    public bool InvertPitch = false;
    private float Pitch = 0.0f;
    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        if (insoleSide != InsoleSide) { return; }

        var latestPitch = quaternion.GetPitch();

        switch (PedalMode)
        {
            case BS_PedalMode.Sustain:
            case BS_PedalMode.Drum:
                var didLatestPitchExceedThreshold = DoesPitchExceedThreshold(latestPitch);
                if (didLatestPitchExceedThreshold != DidPitchExceedThreshold)
                {
                    if (didLatestPitchExceedThreshold && PedalMode == BS_PedalMode.Drum)
                    {
                        PlayDrum();
                    }
                    DidPitchExceedThreshold = didLatestPitchExceedThreshold;
                }
                if (PedalMode == BS_PedalMode.Sustain)
                {
                    Sustain = didLatestPitchExceedThreshold;
                }
                break;
            case BS_PedalMode.Reverb:
            case BS_PedalMode.Chorus:
                var value = PitchRange.UpdateAndGetNormalization(latestPitch, false);
                // FILL - use value to apply effects (effects in Pro version only)
                break;
        }

        Pitch = latestPitch;
    }
    private bool DoesPitchExceedThreshold(float pitch)
    {
        var offset = DidPitchExceedThreshold ? 1.0f : 0.0f;
        return InvertPitch ? pitch < (PitchThreshold + offset) : pitch > (PitchThreshold + offset);
    }
    private bool DidPitchExceedThreshold = false;
    public void Calibrate()
    {
        switch (PedalMode)
        {
            case BS_PedalMode.Sustain:
            case BS_PedalMode.Drum:
                PitchThreshold = Pitch;
                break;
            case BS_PedalMode.Reverb:
            case BS_PedalMode.Chorus:
                PitchRange.Reset();
                break;
        }
    }

    private void Reset()
    {
        PitchRange.Reset();
        Sustain = false;
    }
    // BS END
}
