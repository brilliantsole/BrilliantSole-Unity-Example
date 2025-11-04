using System;
using System.Collections.Generic;
using UnityEngine;

public class BS_PianoTracks : MonoBehaviour
{
    private readonly List<BS_PianoTrack> pianoTracks = new();
    public IReadOnlyList<BS_PianoTrack> PianoTracks => pianoTracks;

    public delegate void OnColumnIsHoveredDelegate(
        BS_PianoTrack track,
        BS_PianoTrackColumn column,
        bool isHovered
    );
    public delegate void OnTrackIsHoveredDelegate(
        BS_PianoTrack track,
        bool isHovered
    );

    public event OnColumnIsHoveredDelegate OnColumnIsHovered;
    public event OnTrackIsHoveredDelegate OnTrackIsHovered;

    private BS_PianoTrack hoveredTrack;
    public BS_PianoTrack HoveredTrack => hoveredTrack;

    private BS_PianoTrackColumn hoveredColumn;
    public BS_PianoTrackColumn HoveredColumn => hoveredColumn;

    private void Start()
    {
        pianoTracks.AddRange(transform.GetComponentsInChildren<BS_PianoTrack>());
        foreach (var track in pianoTracks)
        {
            track.OnColumnIsHovered += (column, isHovered) =>
            {
                if (isHovered)
                {
                    if (hoveredTrack != track)
                    {
                        hoveredTrack = track;
                        OnTrackIsHovered?.Invoke(track, isHovered);
                    }
                }
                else if (track == hoveredTrack)
                {
                    hoveredTrack = null;
                    OnTrackIsHovered?.Invoke(track, isHovered);
                }

                if (isHovered)
                {
                    hoveredColumn = column;
                }
                else if (column == hoveredColumn)
                {
                    hoveredColumn = null;
                }

                OnColumnIsHovered?.Invoke(track, column, isHovered);
            };
        }
    }

    public void AddNote(int midiNote, bool clearRest)
    {
        if (HoveredColumn == null) { return; }
        HoveredColumn.AddNote(midiNote, clearRest);
    }

    public void ClearHoveredColumn()
    {
        if (HoveredColumn == null) { return; }
        HoveredColumn.Clear();
    }

    public void Clear()
    {
        foreach (var track in pianoTracks)
        {
            track.Clear();
        }
    }
}
