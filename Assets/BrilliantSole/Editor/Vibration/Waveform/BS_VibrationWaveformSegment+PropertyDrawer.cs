using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BS_VibrationWaveformSegment))]
public class BS_VibrationWaveformSegmentPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;

        // Calculate field heights
        var durationProperty = property.FindPropertyRelative("Duration");
        var amplitudeProperty = property.FindPropertyRelative("Amplitude");

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, amplitudeProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var durationLabel = new GUIContent($"Duration (ms)");
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, durationProperty, durationLabel);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var durationProperty = property.FindPropertyRelative("Duration");
        var amplitudeProperty = property.FindPropertyRelative("Amplitude");

        // Default height for the duration/amplitude
        float height = 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return height;
    }
}