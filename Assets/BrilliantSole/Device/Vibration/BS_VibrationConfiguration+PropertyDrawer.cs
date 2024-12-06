using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BS_VibrationConfiguration))]
public class BS_VibrationConfigurationPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;

        // Calculate field heights
        var typeProperty = property.FindPropertyRelative("Type");
        var locationsProperty = property.FindPropertyRelative("Locations");
        var waveformEffectSequenceListProperty = property.FindPropertyRelative("WaveformEffectSequence");
        var waveformSequenceListProperty = property.FindPropertyRelative("WaveformSequence");
        var waveformEffectSequenceLoopCountProperty = property.FindPropertyRelative("WaveformEffectSequenceLoopCount");

        // Draw the Type Enum
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, typeProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw the Locations Bitflag
        EditorGUI.PropertyField(position, locationsProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw the corresponding list
        if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.Waveform)
        {
            var waveformSequenceListLabel = new GUIContent($"Sequence (max {BS_VibrationConfiguration.MaxNumberOfWaveformSegments})");
            EditorGUI.PropertyField(position, waveformSequenceListProperty, waveformSequenceListLabel, true);
            position.y += EditorGUI.GetPropertyHeight(waveformSequenceListProperty, true) + EditorGUIUtility.standardVerticalSpacing;
        }
        else if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.WaveformEffect)
        {
            var loopCountLabel = new GUIContent($"Loop Count (0-{BS_VibrationConfiguration.MaxWaveformEffectSequenceLoopCount})");
            EditorGUI.PropertyField(position, waveformEffectSequenceLoopCountProperty, loopCountLabel);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var waveformEffectSequenceListLabel = new GUIContent($"Sequence (max {BS_VibrationConfiguration.MaxNumberOfWaveformEffectSegments})");
            EditorGUI.PropertyField(position, waveformEffectSequenceListProperty, waveformEffectSequenceListLabel, true);
            position.y += EditorGUI.GetPropertyHeight(waveformEffectSequenceListProperty, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProperty = property.FindPropertyRelative("Type");
        var waveformEffectSequenceListProperty = property.FindPropertyRelative("WaveformEffectSequence");
        var waveformSequenceListProperty = property.FindPropertyRelative("WaveformSequence");

        // Default height for the type/locations
        float height = 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Add height for the visible list
        if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.Waveform)
        {
            height += EditorGUI.GetPropertyHeight(waveformSequenceListProperty, true);
        }
        else if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.WaveformEffect)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUI.GetPropertyHeight(waveformEffectSequenceListProperty, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}