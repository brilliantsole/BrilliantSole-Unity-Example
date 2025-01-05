using System.Collections.Generic;
using UnityEngine;

public class BS_PianoTracks : MonoBehaviour
{
    private readonly List<BS_PianoTrack> pianoTracks = new();
    public IReadOnlyList<BS_PianoTrack> PianoTracks => pianoTracks;

    private void Start()
    {
        pianoTracks.AddRange(transform.GetComponentsInChildren<BS_PianoTrack>());
    }
}
