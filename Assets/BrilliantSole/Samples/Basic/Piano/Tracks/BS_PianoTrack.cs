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

    private void Start()
    {
        columns.AddRange(transform.GetComponentsInChildren<BS_PianoTrackColumn>());
        UpdateColumnOctaves();
    }

    private void UpdateColumnOctaves()
    {
        foreach (var column in columns)
        {
            Debug.Log($"updating column octave {Octave}");
            column.Octave = Octave;
        }
    }
}
