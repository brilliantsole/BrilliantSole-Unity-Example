using jp.kshoji.unity.midi;
using UnityEditor;
using UnityEngine;

public class BS_Piano : MonoBehaviour, IMidiDeviceEventHandler, IMidiAllEventsHandler
{
    private void Log(string message)
    {
        Debug.Log(message);
    }
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    private void LogError(string message)
    {
        Debug.LogError(message);
    }

    private void Awake()
    {
        MidiManager.Instance.RegisterEventHandleObject(gameObject);
        MidiManager.Instance.InitializeMidi(() =>
        {
            Debug.Log("initialized midi");
        });
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnDestroy()
    {
        MidiManager.Instance.TerminateMidi();
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            MidiManager.Instance.TerminateMidi();
        }
    }
#endif

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
    }

    public void OnMidiNoteOff(string deviceId, int group, int channel, int note, int velocity)
    {
        Log($"OnMidiNoteOff note: {note}, velocity: {velocity}");
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
}
