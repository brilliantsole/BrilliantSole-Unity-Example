using System;
using System.Collections.Generic;
using UnityEngine;

public class BS_PianoTrack : MonoBehaviour
{
    private readonly List<BS_PianoTrackColumn> columns = new();
    public IReadOnlyList<BS_PianoTrackColumn> Columns => columns;

    [SerializeField]
    private int octave = 4;
    public int Octave
    {
        get => octave;
        set
        {
            if (value == octave) { return; }
            octave = value;
            UpdateColumnOctaves();
        }
    }

    public delegate void OnColumnIsHoveredDelegate(BS_PianoTrackColumn column, bool isHovered);
    public event OnColumnIsHoveredDelegate OnColumnIsHovered;

    private BS_PianoTrackColumn hoveredColumn;
    public BS_PianoTrackColumn HoveredColumn => hoveredColumn;

    private void Start()
    {
        columns.AddRange(transform.GetComponentsInChildren<BS_PianoTrackColumn>());
        foreach (var column in columns)
        {
            column.transform.GetComponent<BS_EyeInteractable>().OnIsHovered += (eyeInteractable, isHovered) =>
            {
                if (isHovered)
                {
                    hoveredColumn = column;
                }
                else if (column == hoveredColumn)
                {
                    hoveredColumn = null;
                }
                OnColumnIsHovered?.Invoke(column, isHovered);
            };
        }
        UpdateColumnOctaves();
    }

    private void UpdateColumnOctaves()
    {
        foreach (var column in columns)
        {
            column.Octave = Octave;
        }
    }

    public void ClearHoveredColumn()
    {
        if (hoveredColumn == null) { return; }
        hoveredColumn.Clear();
    }
    public void Clear()
    {
        foreach (var column in columns)
        {
            column.Clear();
        }
    }
}
