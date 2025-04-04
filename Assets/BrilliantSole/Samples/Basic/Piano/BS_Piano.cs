using System;
using System.Collections.Generic;
using jp.kshoji.unity.midi;
using MidiPlayerTK;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BS_Piano : MonoBehaviour, IMidiDeviceEventHandler, IMidiAllEventsHandler
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_Piano");

    public BS_Side InsoleSide = BS_Side.Right;
    public BS_SensorRate SensorRate = BS_SensorRate._40ms;
    private BS_DevicePair DevicePair => BS_DevicePair.Insoles;
    private BS_Device Device => DevicePair.Devices.ContainsKey(InsoleSide) ? DevicePair.Devices[InsoleSide] : null;
    private bool IsInsoleConnected => Device?.IsConnected == true;

    public GameObject InstrumentsGrid;
    public List<string> InstrumentsGridNames = new();
    public GameObject InstrumentGridButton;

    public BS_PianoUI PianoUI;

    public GameObject Tracks;
    private BS_PianoTracks PianoTracks;
    public GameObject TrackPrefab;

    public enum BS_PedalMode
    {
        None,
        Sustain,
        Reverb,
        Chorus,
        Drum,
        PlayOnHover
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
        PlayOnHoverText.transform.parent.gameObject.SetActive(PedalMode == BS_PedalMode.PlayOnHover);

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

    private void SetPedalMode(BS_PedalMode pedalMode)
    {
        PedalModeDropdown.value = Array.IndexOf(Enum.GetValues(typeof(BS_PedalMode)), pedalMode);
        PedalMode = pedalMode;
    }
    private void SetMode(BS_Mode mode)
    {
        ModeDropdown.value = Array.IndexOf(Enum.GetValues(typeof(BS_Mode)), mode);
        Mode = mode;
    }

    public TMP_Dropdown PedalModeDropdown;
    public Button CalibrateButton;
    public TextMeshProUGUI SustainText;
    public TextMeshProUGUI PlayOnHoverText;

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
        SetPedalMode(PedalMode);
        OnPedalMode();

        ModeDropdown.onValueChanged.AddListener(OnModeDropdownValueChanged);

        CalibrateButton.onClick.AddListener(Calibrate);

        PopulateInstrumentsGrid();

        PianoUI.OnKeyDown += OnKeyDown;
        PianoUI.OnKeyUp += OnKeyUp;
        PianoUI.OnHoveredKeyData += OnHoveredKeyData;

        PianoTracks = Tracks.GetComponent<BS_PianoTracks>();
        PianoTracks.OnColumnIsHovered += OnColumnIsHovered;
        PianoTracks.OnTrackIsHovered += OnTrackIsHovered;

        SetMode(Mode);
        OnMode();
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

    private void OnHoveredKeyData(BS_PianoKeyData pianoKeyData, bool isHovered)
    {
        if (PlayOnHover)
        {
            playHoveredNote(pianoKeyData, isHovered);
        }
    }

    [SerializeField]
    private bool playOnHover = false;
    public bool PlayOnHover
    {
        get => playOnHover;
        private set
        {
            if (value == playOnHover) { return; }
            playOnHover = value;

            Logger.Log($"updated PlayOnHover to {PlayOnHover}");
            PlayOnHoverText.gameObject.SetActive(PlayOnHover);

            switch (Mode)
            {
                case BS_Mode.Play:
                    if (PlayOnHover)
                    {
                        selectHoveredInstrument();
                        if (PianoUI.HoveredKeyData != null)
                        {
                            playHoveredNote(PianoUI.HoveredKeyData, true);
                        }
                    }
                    else
                    {
                        clearHoveredNote();
                    }
                    break;
                case BS_Mode.Track:
                    if (PlayOnHover)
                    {
                        if (PianoTracks.HoveredColumn != null)
                        {
                            playColumn(PianoTracks.HoveredColumn, true);
                        }
                    }
                    else
                    {
                        clearColumn();
                    }
                    break;
            }

        }
    }
    private void OnColumnIsHovered(BS_PianoTrack track, BS_PianoTrackColumn column, bool isHovered)
    {
        if (PlayOnHover)
        {
            playColumn(column, isHovered);
        }

    }
    private void OnTrackIsHovered(BS_PianoTrack track, bool isHovered) { }

    private readonly List<BS_PianoTrackNote> currentlyPlayingNotes = new();
    private void clearColumn()
    {
        foreach (var note in currentlyPlayingNotes)
        {
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
            {
                Command = MPTKCommand.NoteOff,
                Value = note.MidiNote,
                Channel = StreamChannel,
            });
        }
        currentlyPlayingNotes.Clear();
    }
    private void playColumn(BS_PianoTrackColumn column, bool isHovered)
    {
        clearColumn();

        if (isHovered)
        {
            foreach (var note in column.Notes)
            {
                if (!note.IsOn) { continue; }
                midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
                {
                    Command = MPTKCommand.NoteOn,
                    Value = note.MidiNote,
                    Channel = StreamChannel,
                    Duration = -1,
                    Velocity = 80
                });
                currentlyPlayingNotes.Add(note);
            }
        }
    }

    private BS_PianoKeyData currentlyPlayingNote = null;
    private void playHoveredNote(BS_PianoKeyData keyData, bool isHovered)
    {
        clearHoveredNote();
        if (isHovered)
        {
            PianoUI.SimulateKeyEvent(keyData, true);
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
            {
                Command = MPTKCommand.NoteOn,
                Value = keyData.MidiNote,
                Channel = StreamChannel,
                Duration = -1,
                Velocity = 80
            });
            if (keyData.gameObject.TryGetComponent<BS_EyeTrackingButton>(out var eyeTrackingButton))
            {
                eyeTrackingButton.OnPointerDown();
            }
            currentlyPlayingNote = keyData;
        }

    }
    private void clearHoveredNote()
    {
        if (currentlyPlayingNote == null) { return; }
        PianoUI.SimulateKeyEvent(currentlyPlayingNote, false);
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOff,
            Value = currentlyPlayingNote.MidiNote,
            Channel = StreamChannel,
        });
        if (currentlyPlayingNote.gameObject.TryGetComponent<BS_EyeTrackingButton>(out var eyeTrackingButton))
        {
            eyeTrackingButton.OnPointerUp();
        }
        currentlyPlayingNote = null;
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

        PianoUI.SetVisibility(Mode == BS_Mode.Play);
        InstrumentsGrid.SetActive(Mode == BS_Mode.Play);

        Tracks.SetActive(Mode == BS_Mode.Track);
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

    private HashSet<int> downMidiNotes = new();
    private void OnKeyDown(BS_PianoKeyData KeyData)
    {
        Log($"OnKeyDown {KeyData.MidiNote}");
        selectHoveredInstrument();
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = KeyData.MidiNote,
            Channel = StreamChannel,
            Duration = -1,
            Velocity = 80
        });
        PianoTracks.AddNote(KeyData.MidiNote, downMidiNotes.Count == 0);
        downMidiNotes.Add(KeyData.MidiNote);
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
        downMidiNotes.Remove(KeyData.MidiNote);
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
    private int? hoveredInstrumentIndex = null;
    private void selectHoveredInstrument()
    {
        if (hoveredInstrumentIndex == null) { return; }
        InstrumentDropdown.value = (int)hoveredInstrumentIndex;
        OnInstrumentDropdownValueChanged((int)hoveredInstrumentIndex);
        if (instrumentGridButtons.TryGetValue((int)hoveredInstrumentIndex, out var instrumentGridButton))
        {
            if (false)
            {

                var eyeTrackingButton = instrumentGridButton.GetComponentInChildren<BS_EyeTrackingButton>();
                if (eyeTrackingButton != null)
                {
                    eyeTrackingButton.OnPointerDown();
                }
            }
            else
            {
                var button = instrumentGridButton.GetComponentInChildren<Button>();
                if (button != null)
                {
                    var pointer = new PointerEventData(EventSystem.current);
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
                }
            }
        }
        else
        {
            Logger.LogError($"couldn't find gridButton for {hoveredInstrumentIndex}");
        }
        hoveredInstrumentIndex = null;
    }
    private readonly Dictionary<int, GameObject> instrumentGridButtons = new();
    private void PopulateInstrumentsGrid()
    {
        foreach (Transform child in InstrumentsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        var instrumentList = MidiPlayerGlobal.MPTK_ListPreset;
        if (instrumentList != null)
        {
            foreach (var preset in instrumentList)
            {
                var instrumentName = RemoveInstrumentNumber(preset.Label);
                if (InstrumentsGridNames.Contains(instrumentName))
                {
                    var instrumentGridButton = Instantiate(InstrumentGridButton, InstrumentsGrid.transform);
                    instrumentGridButtons.Add(preset.Index, instrumentGridButton);
                    instrumentGridButton.GetComponentInChildren<TextMeshProUGUI>().text = instrumentName;
                    instrumentGridButton.GetComponentInChildren<BS_EyeInteractable>().OnIsHovered += (eyeInteractable, isHovered) =>
                    {
                        if (isHovered)
                        {
                            hoveredInstrumentIndex = preset.Index;
                        }
                        else if (hoveredInstrumentIndex == preset.Index)
                        {
                            hoveredInstrumentIndex = null;
                        }
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

    private readonly int pianoChannel = 0;
    private readonly int padChannel = 9;
    public void OnMidiNoteOn(string deviceId, int group, int channel, int note, int velocity)
    {
        Log($"OnMidiNoteOn note: {note}, velocity: {velocity}, channel: {channel}, group: {group}");
        if (channel == pianoChannel)
        {
            velocity = Math.Max(velocity, 40); // soft velocity
            selectHoveredInstrument();
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
            {
                Command = MPTKCommand.NoteOn,
                Value = note,
                Channel = StreamChannel,
                Velocity = velocity
            });
            PianoUI.OnMidiNote(note, true);
            PianoTracks.AddNote(note, downMidiNotes.Count == 0);
            downMidiNotes.Add(note);
        }
        if (channel == padChannel)
        {
            switch (note)
            {
                case 36: // Pad 1
                    if (mode == BS_Mode.Track)
                    {
                        PlayOnHover = !PlayOnHover;
                    }
                    break;
                case 37: // Pad 2
                    PianoTracks.ClearHoveredColumn();
                    break;
                case 38: // Pad 3
                    PianoTracks.Clear();
                    break;
                case 39: // Pad 4
                    break;
                case 40: // Pad 5
                    break;
                case 41: // Pad 6
                    break;
                case 42: // Pad 7
                    break;
                case 43: // Pad 8
                    break;
            }
        }
    }

    public void OnMidiNoteOff(string deviceId, int group, int channel, int note, int velocity)
    {
        Log($"OnMidiNoteOff note: {note}, velocity: {velocity}, channel: {channel}, group: {group}");
        if (channel == pianoChannel)
        {
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
            {
                Command = MPTKCommand.NoteOff,
                Value = note,
                Channel = StreamChannel,
                //Velocity = velocity
            });
            if (!Sustain)
            {
                PianoUI.OnMidiNote(note, false);
                downMidiNotes.Remove(note);
            }
        }
        if (channel == padChannel)
        {
            switch (note)
            {
                case 36: // Pad 1
                    break;
                case 37: // Pad 2
                    break;
                case 38: // Pad 3
                    break;
                case 39: // Pad 4
                    break;
                case 40: // Pad 5
                    break;
                case 41: // Pad 6
                    break;
                case 42: // Pad 7
                    break;
                case 43: // Pad 8
                    break;
                default:
                    Log($"uncaught pad note {note}");
                    break;
            }
        }
    }

    public void OnMidiChannelAftertouch(string deviceId, int group, int channel, int pressure)
    {
        Log($"OnMidiChannelAftertouch pressure: {pressure}, channel: {channel}, group: {group}");
    }

    public void OnMidiPitchWheel(string deviceId, int group, int channel, int amount)
    {
        Log($"OnMidiPitchWheel channel: {channel}, amount: {amount}, group: {group}");
    }

    public void OnMidiPolyphonicAftertouch(string deviceId, int group, int channel, int note, int pressure)
    {
        Log($"OnMidiPolyphonicAftertouch note: {note}, pressure: {pressure}, channel: {channel}, group: {group}");
    }

    public void OnMidiProgramChange(string deviceId, int group, int channel, int program)
    {
        Log($"OnMidiProgramChange, channel: {channel}, group: {group}, {program}");
    }

    public void OnMidiControlChange(string deviceId, int group, int channel, int function, int value)
    {
        Log($"OnMidiControlChange function: {function}, value: {value}, channel: {channel}, group: {group}");
    }

    public void OnMidiContinue(string deviceId, int group)
    {
        Log($"OnMidiContinue, group: {group}");
    }

    public void OnMidiReset(string deviceId, int group)
    {
        Log($"OnMidiReset, group: {group}");
    }

    public void OnMidiStart(string deviceId, int group)
    {
        Log($"OnMidiStart, group: {group}");
    }

    public void OnMidiStop(string deviceId, int group)
    {
        Log($"OnMidiStop, group: {group}");
    }

    public void OnMidiActiveSensing(string deviceId, int group)
    {
        Log($"OnMidiActiveSensing,  group: {group}");
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
            if (!Sustain)
            {
                foreach (var note in downMidiNotes)
                {
                    PianoUI.OnMidiNote(note, false);
                }
                downMidiNotes.Clear();
            }
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
    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_Side side, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        if (side != InsoleSide) { return; }

        var latestPitch = quaternion.GetPitch();

        switch (PedalMode)
        {
            case BS_PedalMode.Sustain:
            case BS_PedalMode.Drum:
            case BS_PedalMode.PlayOnHover:
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
                if (PedalMode == BS_PedalMode.PlayOnHover)
                {
                    PlayOnHover = didLatestPitchExceedThreshold;
                }
                break;
            case BS_PedalMode.Reverb:
            case BS_PedalMode.Chorus:
                var value = PitchRange.UpdateAndGetNormalization(latestPitch, false);
                // TODO - use value to apply effects (effects in Pro version only)
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
            case BS_PedalMode.PlayOnHover:
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
