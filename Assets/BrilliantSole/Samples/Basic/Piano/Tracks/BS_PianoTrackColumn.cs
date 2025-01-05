using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BS_PianoTrackColumn : BS_BaseEyeTrackingUIComponent, IPointerEnterHandler, IPointerExitHandler
{
    protected Image image;
    protected override void Start()
    {
        base.Start();
        image = GetComponent<Image>();
        UpdateColor();
    }

    public Color Color = Color.white;
    public Color HoverColor = Color.yellow;

    private void UpdateColor()
    {
        image.color = IsHovered ? HoverColor : Color;
    }

    private readonly List<BS_PianoTrackNote> notes = new();
    public IReadOnlyList<BS_PianoTrackNote> Notes => notes;

    private readonly Dictionary<int, BS_PianoTrackNote> midiMapping = new();

    [SerializeField]
    private int octave = 4;
    public int Octave
    {
        get => octave;
        set
        {
            //if (value == octave) { return; }
            octave = value;
            UpdateMidiNotes();
        }
    }

    private void Awake()
    {
        notes.AddRange(transform.GetComponentsInChildren<BS_PianoTrackNote>());
    }

    private void UpdateMidiNotes()
    {
        midiMapping.Clear();
        var midiNote = (octave * 12) + notes.Count;
        foreach (var note in notes)
        {
            midiNote--;
            note.MidiNote = midiNote;
            midiMapping[midiNote] = note;
        }
    }

    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        foreach (var note in notes)
        {
            note.IsHovered = IsHovered;
        }
        UpdateColor();
    }

    public void SetMidiOn(bool isOn)
    {
        foreach (var note in notes)
        {
            note.IsOn = isOn;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        eyeInteractable.SetIsHovered(true);
#else
        if (eventData.pointerId == BS_EyeTrackingCanvas.PointerId)
        {
            eyeInteractable.SetIsHovered(true);
        }
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {

#if UNITY_EDITOR
        eyeInteractable.SetIsHovered(false);
#else
        if (eventData.pointerId == BS_EyeTrackingCanvas.PointerId)
        {
            eyeInteractable.SetIsHovered(false);
        }
#endif

    }

    public void AddNote(int midiNote, bool clearRest)
    {
        if (clearRest)
        {
            foreach (var _note in notes)
            {
                _note.IsOn = false;
            }
        }

        if (midiMapping.TryGetValue(midiNote, out var note))
        {
            note.IsOn = true;
        }
        else
        {
            Debug.Log($"no note found for {midiNote}");
        }
    }
}
