using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BS_PianoUI))]
public class PianoUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BS_PianoUI piano = (BS_PianoUI)target;
        if (GUILayout.Button("Generate Keys"))
        {
            piano.GenerateKeys();
        }
    }
}