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
        var waveformEffectSequenceListProperty = property.FindPropertyRelative("WaveformEffectSequence");
        var waveformSequenceListProperty = property.FindPropertyRelative("WaveformSequence");

        // Draw the Type Enum
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, typeProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw the corresponding list
        if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.Waveform)
        {
            EditorGUI.PropertyField(position, waveformSequenceListProperty, true);
        }
        else if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.WaveformEffect)
        {
            EditorGUI.PropertyField(position, waveformEffectSequenceListProperty, true);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProperty = property.FindPropertyRelative("Type");
        var waveformEffectSequenceListProperty = property.FindPropertyRelative("WaveformEffectSequence");
        var waveformSequenceListProperty = property.FindPropertyRelative("WaveformSequence");

        // Default height for the type enum
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Add height for the visible list
        if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.Waveform)
        {
            height += EditorGUI.GetPropertyHeight(waveformSequenceListProperty, true);
        }
        else if ((BS_VibrationType)typeProperty.enumValueIndex == BS_VibrationType.WaveformEffect)
        {
            height += EditorGUI.GetPropertyHeight(waveformEffectSequenceListProperty, true);
        }

        return height;
    }
}