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

    private void UpdateColor()
    {
        image.enabled = IsHovered;
    }

    private readonly List<BS_PianoTrackNote> notes = new();
    public IReadOnlyList<BS_PianoTrackNote> Notes => notes;

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
        var midiNote = (octave * 12) + notes.Count;
        foreach (var note in notes)
        {
            midiNote--;
            note.MidiNote = midiNote;
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
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_EDITOR
        eyeInteractable.SetIsHovered(false);
#endif
    }
}
