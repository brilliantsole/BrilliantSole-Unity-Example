using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BS_PianoTrackNote : MonoBehaviour
{
    protected Image image;
    private void Start()
    {
        image = GetComponent<Image>();
        UpdateColor();
    }

    public int MidiNote;

    [SerializeField]
    private bool isWhite;
    public bool IsWhite
    {
        get => isWhite;
        set
        {
            if (value == isWhite) { return; }
            isWhite = value;
            UpdateColor();
        }
    }

    [SerializeField]
    private bool isOn;
    public bool IsOn
    {
        get => isOn;
        set
        {
            if (value == isOn) { return; }
            isOn = value;
            UpdateColor();
        }
    }

    [SerializeField]
    private bool isHovered;
    public bool IsHovered
    {
        get => isHovered;
        set
        {
            if (value == isHovered) { return; }
            isHovered = value;
            UpdateColor();
        }
    }

    public BS_PianoTrackNoteColors PianoTrackNoteColors;

    public void UpdateColor()
    {
        image.color = PianoTrackNoteColors.GetColor(IsWhite, IsOn, IsHovered);
    }
}