using UnityEngine;

public class BS_PianoKeyData : MonoBehaviour
{

    public int MidiNote;
    public int TruncatedMidiNote;

    public enum BS_PianoKeyColor
    {
        White,
        Black
    }
    public BS_PianoKeyColor Color;
    public bool IsDown;
}
